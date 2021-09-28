using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Isu.Tools;

namespace IsuExtra.Entities
{
    public class Timetable
    {
        private static readonly int _daysCount = 7;
        private List<Day> _days;

        public Timetable(List<Day> days)
        {
            if (days.Count != _daysCount)
                throw new IsuException("There must be 7 days in the schedule");
            _days = new List<Day>(days);
        }

        public Timetable()
        {
            _days = Enumerable.Repeat(new Day(), _daysCount).ToList();
        }

        public IReadOnlyList<Day> Days => _days;

        public bool CheckIntersection(GroupTimetable timetable) =>
            (from day in _days from timetableDay in timetable.Timetable.Days where day.CheckIntersection(timetableDay) select day).Any();
    }
}