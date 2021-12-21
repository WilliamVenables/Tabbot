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


        public CreateHabit() {
            InitializeComponent();

            DataContext = this;
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
                Trace.WriteLine(App.dbPath);
                connection.CreateTable<Habit>();
                connection.Insert(habit);
            }

            Close();
        }
    }
}
