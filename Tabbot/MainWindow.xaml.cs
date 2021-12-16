using Microsoft.Toolkit.Uwp.Notifications;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Tabbot {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    /*new ToastContentBuilder()
            .AddText("Hey there!")
            .AddText("It's time to study Swedish")
            .AddButton(new ToastButton()
                .SetContent("Working on it")
                .AddArgument("action", "startTimer")
                .SetBackgroundActivation())
            .AddButton(new ToastButton()
                .SetContent("Done")
                .AddArgument("action", "done")
                .AddArgument("arg", "1234")
                .SetBackgroundActivation())
                .Show();*/

    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void CreateHabit(object sender, RoutedEventArgs e) {
            CreateHabit popup = new();
            popup.ShowDialog();
            /*Days days = Days.Monday | Days.Tuesday | Days.Thursday | Days.Friday;

            Habit habit = new Habit("Study Swedish", "Study Swedish for 30 mins using the book and Quizlet", days, new TimeSpan(12, 0, 0), 30);

            using (SQLiteConnection connection = new SQLiteConnection(App.dbPath)) {
                Trace.WriteLine(App.dbPath);
                connection.CreateTable<Habit>();
                connection.Insert(habit);

                var query = connection.Table<Habit>();

                foreach (var _habit in query)
                    Trace.WriteLine("Time to " + _habit.Title);
            }*/
        }
    }
}
