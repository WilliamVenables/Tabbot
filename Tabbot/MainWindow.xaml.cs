using Microsoft.Toolkit.Uwp.Notifications;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Windows.UI.Notifications;

namespace Tabbot {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    /*new ToastContentBuilder()
            .AddText("Hey there!")
            .AddText("It's time to study Swedish")
            .AddButton(new ToastButton()
                .SetContent("Snooze")
                .AddArgument("action", "delay")
                .SetBackgroundActivation())
            .AddButton(new ToastButton()
                .SetContent("Start")
                .AddArgument("action", "startTimer")
                .SetBackgroundActivation())
                .Show();*/

    public partial class MainWindow : Window {
        private Dictionary<int, Habit> habits = new();

        public MainWindow() {
            InitializeComponent();

            UpdateHabits();

            ScheduleNextHabit();
        }

        private void CreateHabit(object sender, RoutedEventArgs e) {
            CreateHabit popup = new();

            popup.ShowDialog();

            UpdateHabits();

            ScheduleNextHabit();
        }

        private void UpdateHabits() {
            StackPanel allTab = (StackPanel)this.FindName("sp_habitsAll");
            allTab.Children.Clear();

            habits.Clear();

            using (SQLiteConnection connection = new(App.dbPath)) {
                connection.CreateTable<Habit>();

                var query = connection.Table<Habit>();

                foreach (var habit in query) {
                    habits.Add(habit.Id, habit);

                    Grid container = new();
                    container.Background = this.Resources["TabbotForeground"] as Brush;
                    container.Margin = new Thickness(10);

                    RowDefinition rowDef1 = new();
                    RowDefinition rowDef2 = new();
                    RowDefinition rowDef3 = new();
                    RowDefinition rowDef4 = new();

                    container.RowDefinitions.Add(rowDef1);
                    container.RowDefinitions.Add(rowDef2);
                    container.RowDefinitions.Add(rowDef3);
                    container.RowDefinitions.Add(rowDef4);

                    TextBlock id = new() {
                        Visibility = Visibility.Collapsed,
                        Text = habit.Id.ToString()
                    };

                    TextBlock txt_title = new() {
                        Foreground = this.Resources["TabbotText"] as Brush,
                        Margin = new Thickness(4, 0, 4, 0),
                        FontSize = 18,
                        FontWeight = FontWeights.Bold,
                        Text = habit.Title
                    };
                    Grid.SetRow(txt_title, 0);

                    Separator separator = new() {
                        Background = this.Resources["TabbotAccent"] as Brush,
                        Margin = new Thickness(2, 0, 2, 0)
                    };
                    Grid.SetRow(separator, 1);

                    TextBlock txt_desc = new() {
                        Foreground = this.Resources["TabbotText"] as Brush,
                        Margin = new Thickness(4, 0, 4, 0),
                        FontSize = 18,
                        Text = habit.Description,
                        TextWrapping = TextWrapping.WrapWithOverflow
                    };
                    Grid.SetRow(txt_desc, 2);

                    TextBlock txt_start = new() {
                        Foreground = this.Resources["TabbotText"] as Brush,
                        Margin = new Thickness(4, 8, 4, 8),
                        FontSize = 18
                    };
                    txt_start.Inlines.Add(new Bold(new Run("Starts at: ")));
                    DateTime time = DateTime.Today.Add(habit.TimeOfDay);
                    txt_start.Inlines.Add(time.ToString("h:mm tt"));
                    Grid.SetRow(txt_start, 3);

                    container.Children.Add(id);
                    container.Children.Add(txt_title);
                    container.Children.Add(separator);
                    container.Children.Add(txt_desc);
                    container.Children.Add(txt_start);

                    allTab.Children.Add(container);
                }

            }
        }

        private void ScheduleNextHabit() {
            // Create the toast notifier
            ToastNotifierCompat notifier = ToastNotificationManagerCompat.CreateToastNotifier();

            // Get the list of scheduled toasts that haven't appeared yet
            IReadOnlyList<ScheduledToastNotification> scheduledToasts = notifier.GetScheduledToastNotifications();

            foreach (ScheduledToastNotification notification in scheduledToasts) {
                if (notification.Group == "Habit Reminder") {
                    notifier.RemoveFromSchedule(notification);
                }
            }

            DateTime now = DateTime.Now;

            int id = -1;
            TimeSpan nextHabitTime = TimeSpan.MaxValue;

            int attempts = 0;

            do {
                foreach (Habit h in habits.Values) {
                    if (h.DaysOfTheWeek.HasFlag((Days)Enum.Parse(typeof(Days), now.AddDays(attempts).DayOfWeek.ToString()))) {
                        if (h.TimeOfDay < nextHabitTime && DateTime.Today.Add(h.TimeOfDay) > now) {
                            nextHabitTime = h.TimeOfDay;

                            id = h.Id;
                        }
                    }
                }

                ++attempts;
            } while (id < 0 && attempts < 7);

            if (id > -1) {
                Habit nextHabit = habits[id];

                TimeSpan secondsUntilHabit = DateTime.Today.Add(nextHabit.TimeOfDay) - DateTime.Now;

                new ToastContentBuilder()
                    .AddText("Hey there!")
                    .AddText("It's time to " + nextHabit.Title)
                    .AddText(nextHabit.Description)
                    .AddToastInput(new ToastSelectionBox("snoozeTime") {
                        DefaultSelectionBoxItemId = "5",
                        Items =
                            {
                                new ToastSelectionBoxItem("5", "5 minutes"),
                                new ToastSelectionBoxItem("15", "15 minutes"),
                                new ToastSelectionBoxItem("30", "30 minutes"),
                                new ToastSelectionBoxItem("60", "1 hour"),
                            }
                    })
                    .AddButton(new ToastButtonSnooze() { SelectionBoxId = "snoozeTime" })
                    .AddButton(new ToastButton()
                        .SetContent("Start")
                        .AddArgument("action", "startTimer")
                        .AddArgument("duration", nextHabit.Duration)
                        .AddArgument("title", nextHabit.Title)
                        .SetBackgroundActivation())
                    .SetToastScenario(ToastScenario.Reminder)
                    .Schedule(DateTimeOffset.Now + TimeSpan.FromSeconds(secondsUntilHabit.TotalSeconds), toast => {
                        toast.Group = "Habit Reminder";
                    });
            }
        }
    }
}
