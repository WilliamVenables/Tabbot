using SQLite;
using System;

namespace Tabbot {
    [Flags]
    public enum Days : byte {
        None = 0,
        Sunday = 1,
        Monday = 2,
        Tuesday = 4,
        Wednesday = 8,
        Thursday = 16,
        Friday = 32,
        Saturday = 64
    };

    public enum Completion : byte {
        Incomplete,
        WIP,
        Done
    }

    internal class Habit {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Title { get; set; } = "";

        public string Description { get; set; } = "";

        public Days DaysOfTheWeek { get; set; }

        public TimeSpan TimeOfDay { get; set; }

        public int Duration { get; set; }

        public Completion Completion { get; set; }

        public Habit(string _title, string _desc, Days _days, TimeSpan _time, int _duration) {
            Title = _title;
            Description = _desc;
            DaysOfTheWeek = _days;
            TimeOfDay = _time;
            Duration = _duration;
            Completion = Completion.Incomplete;
        }

        public Habit() {

        }
    }
}
