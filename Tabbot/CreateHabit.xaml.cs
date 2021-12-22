using Microsoft.Toolkit.Uwp.Notifications;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Tabbot {
    /// <summary>
    /// Interaction logic for CreateHabit.xaml
    /// </summary>
    public partial class CreateHabit : Window {
        bool create = true;
        int updateID = 0;

        public CreateHabit() {
            InitializeComponent();

            create = true;

            DataContext = this;
        }

        public CreateHabit(int id) {
            InitializeComponent();

            create = false;
            updateID = id;

            using (SQLiteConnection connection = new(App.dbPath)) {
                Habit h = connection.Get<Habit>(updateID);

                TitleBox.Text = h.Title;
                DescBox.Text = h.Description;

                cb_sun.IsChecked = h.DaysOfTheWeek.HasFlag(Days.Sunday);
                cb_mon.IsChecked = h.DaysOfTheWeek.HasFlag(Days.Monday);
                cb_tue.IsChecked = h.DaysOfTheWeek.HasFlag(Days.Tuesday);
                cb_wed.IsChecked = h.DaysOfTheWeek.HasFlag(Days.Wednesday);
                cb_thu.IsChecked = h.DaysOfTheWeek.HasFlag(Days.Thursday);
                cb_fri.IsChecked = h.DaysOfTheWeek.HasFlag(Days.Friday);
                cb_sat.IsChecked = h.DaysOfTheWeek.HasFlag(Days.Saturday);

                tp_start.Value = DateTime.Today.AddHours(h.TimeOfDay.Hours).AddMinutes(h.TimeOfDay.Minutes);

                num_duration.Value = h.Duration;
            }
        }

        private void btn_habit_Click(object sender, RoutedEventArgs e) {
            Days days = (bool)cb_sun.IsChecked ? Days.Sunday : 0;
            days |= (bool)cb_mon.IsChecked ? Days.Monday : 0;
            days |= (bool)cb_tue.IsChecked ? Days.Tuesday : 0;
            days |= (bool)cb_wed.IsChecked ? Days.Wednesday : 0;
            days |= (bool)cb_thu.IsChecked ? Days.Thursday : 0;
            days |= (bool)cb_fri.IsChecked ? Days.Friday : 0;
            days |= (bool)cb_sat.IsChecked ? Days.Saturday : 0;

            Habit habit = new(TitleBox.Text, DescBox.Text, days, new TimeSpan(tp_start.Value.Value.Hour, tp_start.Value.Value.Minute, 0), num_duration.Value.Value);

            using (SQLiteConnection connection = new(App.dbPath)) {
                connection.CreateTable<Habit>();

                if (create) {
                    connection.Insert(habit);
                } else {
                    var query = connection.Table<Habit>().Where(c => c.Id == updateID).SingleOrDefault();
                    query.Title = habit.Title;
                    query.Description = habit.Description;
                    query.DaysOfTheWeek = habit.DaysOfTheWeek;
                    query.TimeOfDay = habit.TimeOfDay;
                    query.Duration = habit.Duration;

                    connection.Update(query);
                }
            }

            Close();
        }
    }
}
