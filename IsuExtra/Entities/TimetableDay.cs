using System.Collections.Generic;
using System.Linq;
using Isu.Tools;

namespace IsuExtra.Entities
{
    public class TimetableDay
    {
        private List<Lesson> _lessons;

        public TimetableDay(List<Lesson> lessons)
        {
            _lessons = new List<Lesson>(lessons);
        }

        public TimetableDay()
            : this(new List<Lesson>()) { }

        public IReadOnlyList<Lesson> Lessons => _lessons;

        public bool CheckIntersection(Lesson lesson) => _lessons.Any(lsn => lesson.CheckIntersection(lsn));

        public bool CheckIntersection(TimetableDay timetableDay) =>
            _lessons.Any(day => timetableDay._lessons.Any(day.CheckIntersection));

        internal void Add(Lesson lesson)
        {
            if (CheckIntersection(lesson))
                throw new IsuException("Lessons cannot overlap");
            _lessons.Add(lesson);
            _lessons.Sort();
        }

        internal void Remove(Lesson lesson)
        {
            _lessons.Remove(lesson);
        }
    }
}