using Microsoft.Toolkit.Uwp.Notifications;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.UI.Notifications;

namespace Tabbot {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

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
            StackPanel todayTab = (StackPanel)this.FindName("sp_habitsToday");

            allTab.Children.Clear();
            todayTab.Children.Clear();

            habits.Clear();

            using (SQLiteConnection connection = new(App.dbPath)) {
                connection.CreateTable<Habit>();

                var query = connection.Table<Habit>();

                foreach (var habit in query) {
                    habits.Add(habit.Id, habit);

                    #region All
                    {
                        Grid container = new();
                        container.Background = Application.Current.Resources["TabbotForeground"] as Brush;
                        container.Margin = new Thickness(10);

                        container.RowDefinitions.Add(new());
                        container.RowDefinitions.Add(new());
                        container.RowDefinitions.Add(new());
                        container.RowDefinitions.Add(new());
                        container.RowDefinitions.Add(new());

                        container.ColumnDefinitions.Add(new() { Width = new GridLength(100, GridUnitType.Star) });
                        container.ColumnDefinitions.Add(new() { Width = new GridLength(30) });
                        container.ColumnDefinitions.Add(new() { Width = new GridLength(30) });

                        TextBlock txt_title = new() {
                            Foreground = Application.Current.Resources["TabbotText"] as Brush,
                            Margin = new Thickness(4, 0, 4, 0),
                            FontSize = 20,
                            FontWeight = FontWeights.Bold,
                            Text = habit.Title
                        };
                        Grid.SetRow(txt_title, 0);

                        Button btn_editHabit = new() {
                            Foreground = Application.Current.Resources["TabbotText"] as Brush,
                            Background = Brushes.Transparent,
                            BorderBrush = Brushes.Transparent,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            Padding = new Thickness(4, 0, 4, 0),
                            FontSize = 16,
                            Style = Application.Current.Resources["AddHabitStyle"] as Style,
                            Content = new Image {
                                Source = (BitmapImage)FindResource("TabbotEdit"),
                                Width = 18,
                                Height = 18
                            },
                            Tag = habit.Id
                        };
                        Grid.SetRow(btn_editHabit, 0);
                        Grid.SetColumn(btn_editHabit, 1);
                        btn_editHabit.Click += new RoutedEventHandler(EditHabit);

                        Button btn_deleteHabit = new() {
                            Foreground = Application.Current.Resources["TabbotText"] as Brush,
                            Background = Brushes.Transparent,
                            BorderBrush = Brushes.Transparent,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            Padding = new Thickness(4, 0, 4, 0),
                            Style = Application.Current.Resources["AddHabitStyle"] as Style,
                            Content = new Image {
                                Source = (BitmapImage)FindResource("TabbotClose"),
                                Width = 18,
                                Height = 18
                            },
                            Tag = habit.Id
                        };
                        Grid.SetRow(btn_deleteHabit, 0);
                        Grid.SetColumn(btn_deleteHabit, 2);
                        btn_deleteHabit.Click += new RoutedEventHandler(DeleteHabit);

                        Separator separator = new() {
                            Background = Application.Current.Resources["TabbotAccent"] as Brush,
                            Margin = new Thickness(2, 0, 2, 0)
                        };
                        Grid.SetRow(separator, 1);
                        Grid.SetColumnSpan(separator, 3);

                        TextBlock txt_desc = new() {
                            Foreground = Application.Current.Resources["TabbotText"] as Brush,
                            Margin = new Thickness(4, 0, 4, 0),
                            FontSize = 16,
                            Text = habit.Description,
                            TextWrapping = TextWrapping.WrapWithOverflow
                        };
                        Grid.SetRow(txt_desc, 2);
                        Grid.SetColumnSpan(txt_desc, 3);

                        TextBlock txt_start = new() {
                            Foreground = Application.Current.Resources["TabbotText"] as Brush,
                            Margin = new Thickness(4, 8, 4, 0),
                            FontSize = 16
                        };

                        txt_start.Inlines.Add(new Bold(new Run("Starts at: ")));
                        DateTime time = DateTime.Today.Add(habit.TimeOfDay);
                        txt_start.Inlines.Add(time.ToString("h:mm tt"));
                        Grid.SetRow(txt_start, 3);
                        Grid.SetColumnSpan(txt_start, 3);

                        TextBlock txt_days = new() {
                            Foreground = Application.Current.Resources["TabbotText"] as Brush,
                            Margin = new Thickness(4, 0, 4, 0),
                            FontSize = 16
                        };

                        foreach (Days value in Enum.GetValues(habit.DaysOfTheWeek.GetType())) {
                            if (value == Days.None) {
                                continue;
                            }
                            if (habit.DaysOfTheWeek.HasFlag(value)) {
                                txt_days.Inlines.Add(new Bold(new Run(value.ToString().Substring(0, 1))));
                            }
                            else {
                                txt_days.Inlines.Add(value.ToString().Substring(0, 1));
                            }
                        }
                        Grid.SetRow(txt_days, 4);
                        Grid.SetColumnSpan(txt_desc, 3);

                        container.Children.Add(txt_title);
                        container.Children.Add(btn_editHabit);
                        container.Children.Add(btn_deleteHabit);
                        container.Children.Add(separator);
                        container.Children.Add(txt_desc);
                        container.Children.Add(txt_start);
                        container.Children.Add(txt_days);

                        allTab.Children.Add(container);
                    }
                    #endregion

                    #region Today
                    if (habit.DaysOfTheWeek.HasFlag((Days)Enum.Parse(typeof(Days), DateTime.Today.DayOfWeek.ToString()))) {
                        Grid container = new();
                        container.Background = Application.Current.Resources["TabbotForeground"] as Brush;
                        container.Margin = new Thickness(10);

                        container.RowDefinitions.Add(new());
                        container.RowDefinitions.Add(new());
                        container.RowDefinitions.Add(new());
                        container.RowDefinitions.Add(new());
                        container.RowDefinitions.Add(new());

                        container.ColumnDefinitions.Add(new() { Width = new GridLength(100, GridUnitType.Star) });
                        container.ColumnDefinitions.Add(new() { Width = new GridLength(30) });
                        container.ColumnDefinitions.Add(new() { Width = new GridLength(30) });

                        TextBlock txt_title = new() {
                            Foreground = Application.Current.Resources["TabbotText"] as Brush,
                            Margin = new Thickness(4, 0, 4, 0),
                            FontSize = 20,
                            FontWeight = FontWeights.Bold,
                            Text = habit.Title
                        };
                        Grid.SetRow(txt_title, 0);

                        Button btn_editHabit = new() {
                            Foreground = Application.Current.Resources["TabbotText"] as Brush,
                            Background = Brushes.Transparent,
                            BorderBrush = Brushes.Transparent,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            Padding = new Thickness(4, 0, 4, 0),
                            FontSize = 16,
                            Style = Application.Current.Resources["AddHabitStyle"] as Style,
                            Content = new Image {
                                Source = (BitmapImage)FindResource("TabbotEdit"),
                                Width = 18,
                                Height = 18
                            },
                            Tag = habit.Id
                        };
                        Grid.SetRow(btn_editHabit, 0);
                        Grid.SetColumn(btn_editHabit, 1);
                        btn_editHabit.Click += new RoutedEventHandler(EditHabit);

                        Button btn_deleteHabit = new() {
                            Foreground = Application.Current.Resources["TabbotText"] as Brush,
                            Background = Brushes.Transparent,
                            BorderBrush = Brushes.Transparent,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            Padding = new Thickness(4, 0, 4, 0),
                            Style = Application.Current.Resources["AddHabitStyle"] as Style,
                            Content = new Image {
                                Source = (BitmapImage)FindResource("TabbotClose"),
                                Width = 18,
                                Height = 18
                            },
                            Tag = habit.Id
                        };
                        Grid.SetRow(btn_deleteHabit, 0);
                        Grid.SetColumn(btn_deleteHabit, 2);
                        btn_deleteHabit.Click += new RoutedEventHandler(DeleteHabit);

                        Separator separator = new() {
                            Background = Application.Current.Resources["TabbotAccent"] as Brush,
                            Margin = new Thickness(2, 0, 2, 0)
                        };
                        Grid.SetRow(separator, 1);
                        Grid.SetColumnSpan(separator, 3);

                        TextBlock txt_desc = new() {
                            Foreground = Application.Current.Resources["TabbotText"] as Brush,
                            Margin = new Thickness(4, 0, 4, 0),
                            FontSize = 16,
                            Text = habit.Description,
                            TextWrapping = TextWrapping.WrapWithOverflow
                        };
                        Grid.SetRow(txt_desc, 2);
                        Grid.SetColumnSpan(txt_desc, 3);

                        TextBlock txt_start = new() {
                            Foreground = Application.Current.Resources["TabbotText"] as Brush,
                            Margin = new Thickness(4, 8, 4, 0),
                            FontSize = 16
                        };

                        txt_start.Inlines.Add(new Bold(new Run("Starts at: ")));
                        DateTime time = DateTime.Today.Add(habit.TimeOfDay);
                        txt_start.Inlines.Add(time.ToString("h:mm tt"));
                        Grid.SetRow(txt_start, 3);
                        Grid.SetColumnSpan(txt_start, 3);

                        TextBlock txt_days = new() {
                            Foreground = Application.Current.Resources["TabbotText"] as Brush,
                            Margin = new Thickness(4, 0, 4, 0),
                            FontSize = 16
                        };

                        foreach (Days value in Enum.GetValues(habit.DaysOfTheWeek.GetType())) {
                            if (value == Days.None) {
                                continue;
                            }
                            if (habit.DaysOfTheWeek.HasFlag(value)) {
                                txt_days.Inlines.Add(new Bold(new Run(value.ToString().Substring(0, 1))));
                            }
                            else {
                                txt_days.Inlines.Add(value.ToString().Substring(0, 1));
                            }
                        }
                        Grid.SetRow(txt_days, 4);
                        Grid.SetColumnSpan(txt_desc, 3);

                        container.Children.Add(txt_title);
                        container.Children.Add(btn_editHabit);
                        container.Children.Add(btn_deleteHabit);
                        container.Children.Add(separator);
                        container.Children.Add(txt_desc);
                        container.Children.Add(txt_start);
                        container.Children.Add(txt_days);

                        todayTab.Children.Add(container);
                    }
                    #endregion
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
    
        private void DeleteHabit(object sender, RoutedEventArgs e) {
            int habitId = Convert.ToInt32(((Button)sender).Tag);

            using (SQLiteConnection connection = new(App.dbPath)) {
                Habit toDelete = new Habit();
                toDelete.Id = habitId;

                connection.Delete(toDelete);
            }

            UpdateHabits();
        }
    
        private void EditHabit(object sender, RoutedEventArgs e) {
            int habitId = Convert.ToInt32(((Button)sender).Tag);

            CreateHabit popup = new(habitId);

            popup.ShowDialog();

            UpdateHabits();

            ScheduleNextHabit();
        }
    }
}
