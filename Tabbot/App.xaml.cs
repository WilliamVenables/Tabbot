using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.IO;
using System.Linq;
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
                    string[] args = toastArgs.Argument.Split(';');
                    switch (args[0]) {
                        case "action=startTimer":
                            new ToastContentBuilder()
                                .AddText("You did it!")
                                .AddText("You completed today's goal to " + args[2].Split('=')[1])
                                .Schedule(DateTimeOffset.Now + TimeSpan.FromMinutes(Convert.ToInt32(args[1].Split('=')[1])), toast => {
                                    toast.Group = "Habit Timer";
                                });
                            break;
                        default:
                            MessageBox.Show("Something went wrong.  Please send a bug report with code: " + args[0], "Error");
                            break;
                    }
                });
            };

            base.OnStartup(e);
        }
    }
}
