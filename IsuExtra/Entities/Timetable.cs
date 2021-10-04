using System.Collections.Generic;
using System.Linq;
using Isu.Tools;

namespace IsuExtra.Entities
{
    public class Timetable
    {
        private static readonly int _daysCount = 7;
        private List<TimetableDay> _days;

        public Timetable(List<TimetableDay> days)
        {
            if (days.Count != _daysCount)
                throw new IsuException("There must be 7 days in the schedule");
            _days = new List<TimetableDay>(days);
        }

        public Timetable()
        {
            _days = Enumerable.Repeat(new TimetableDay(), _daysCount).ToList();
        }

        public IReadOnlyList<TimetableDay> Days => _days;

        public bool CheckIntersection(GroupTimetable timetable) =>
            _days.Any(day => timetable.Timetable.Days.Any(day.CheckIntersection));
    }
}