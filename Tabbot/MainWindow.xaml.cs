using SQLite;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

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
        public MainWindow() {
            InitializeComponent();
            
            UpdateHabits();
        }

        private void CreateHabit(object sender, RoutedEventArgs e) {
            CreateHabit popup = new();

            popup.ShowDialog();

            UpdateHabits();
        }

        private void UpdateHabits() {
            using (SQLiteConnection connection = new(App.dbPath)) {
                connection.CreateTable<Habit>();

                var query = connection.Table<Habit>();

                foreach (var habit in query) {
                    Grid container = new Grid();
                    container.Background = this.Resources["TabbotForeground"] as Brush;
                    container.Margin = new Thickness(10);
                    
                    RowDefinition rowDef1 = new RowDefinition();
                    RowDefinition rowDef2 = new RowDefinition();
                    RowDefinition rowDef3 = new RowDefinition();
                    RowDefinition rowDef4 = new RowDefinition();

                    container.RowDefinitions.Add(rowDef1);
                    container.RowDefinitions.Add(rowDef2);
                    container.RowDefinitions.Add(rowDef3);
                    container.RowDefinitions.Add(rowDef4);

                    TextBlock id = new TextBlock();
                    id.Visibility = Visibility.Collapsed;
                    id.Text = habit.Id.ToString();

                    TextBlock txt_title = new TextBlock();
                    txt_title.Foreground = this.Resources["TabbotText"] as Brush;
                    txt_title.Margin = new Thickness(4, 0, 4, 0);
                    txt_title.FontSize = 18;
                    txt_title.FontWeight = FontWeights.Bold;
                    txt_title.Text = habit.Title;
                    Grid.SetRow(txt_title, 0);

                    Separator separator = new Separator();
                    separator.Background = this.Resources["TabbotAccent"] as Brush;
                    separator.Margin = new Thickness(2, 0, 2, 0);
                    Grid.SetRow(separator, 1);

                    TextBlock txt_desc = new TextBlock();
                    txt_desc.Foreground = this.Resources["TabbotText"] as Brush;
                    txt_desc.Margin = new Thickness(4, 0, 4, 0);
                    txt_desc.FontSize = 18;
                    txt_desc.Text = habit.Description;
                    txt_desc.TextWrapping = TextWrapping.WrapWithOverflow;
                    Grid.SetRow(txt_desc, 2);

                    TextBlock txt_start = new TextBlock();
                    txt_start.Foreground = this.Resources["TabbotText"] as Brush;
                    txt_start.Margin = new Thickness(4, 8, 4, 8);
                    txt_start.FontSize = 18;
                    txt_start.Inlines.Add(new Bold(new Run("Starts at: ")));
                    DateTime time = DateTime.Today.Add(habit.TimeOfDay);
                    txt_start.Inlines.Add(time.ToString("h:mm tt"));
                    Grid.SetRow(txt_start, 3);

                    container.Children.Add(txt_title);
                    container.Children.Add(separator);
                    container.Children.Add(txt_desc);
                    container.Children.Add(txt_start);

                    StackPanel allTab = (StackPanel)this.FindName("sp_habitsAll");
                    allTab.Children.Add(container);
                }
                    
            }
        }
    }
}
