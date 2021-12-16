using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.IO;
using System.Windows;
using Windows.Foundation.Collections;

namespace Tabbot {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        static readonly string dbName = "Habits.db";
        static readonly string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Tabbot";
        
        public static readonly string dbPath = Path.Combine(folderPath, dbName);

        protected override void OnStartup(StartupEventArgs e) {
            Directory.CreateDirectory(folderPath);

            // Listen to notification activation
            ToastNotificationManagerCompat.OnActivated += toastArgs => {
                // Obtain the arguments from the notification
                ToastArguments args = ToastArguments.Parse(toastArgs.Argument);

                // Obtain any user input (text boxes, menu selections) from the notification
                ValueSet userInput = toastArgs.UserInput;

                // Need to dispatch to UI thread if performing UI operations
                Application.Current.Dispatcher.Invoke(delegate {
                    // TODO: Show the corresponding content
                    MessageBox.Show("Toast activated. Arg 1: " + toastArgs.Argument.Split(';')[1]);
                });
            };

            base.OnStartup(e);
        }
    }
}
