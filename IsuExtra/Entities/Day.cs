using System;
using System.Collections.Generic;
using System.Linq;
using Isu.Tools;

namespace IsuExtra.Entities
{
    public class Day
    {
        private List<Lesson> _lessons;

        public Day(List<Lesson> lessons)
        {
            _lessons = new List<Lesson>(lessons);
        }

        public Day()
        {
            _lessons = new List<Lesson>();
        }

        public IReadOnlyList<Lesson> Lessons => _lessons;

        public bool CheckIntersection(Lesson lesson) => _lessons.Any(lsn => lesson.CheckIntersection(lsn));

        public bool CheckIntersection(Day day) =>
            (from lsn in _lessons from lsn2 in day._lessons where lsn.CheckIntersection(lsn2) select lsn).Any();

        internal void Add(Lesson lesson)
        {
            if (CheckIntersection(lesson))
                throw new IsuException("Lessons cannot overlap");
            _lessons.Add(lesson);
            _lessons.Sort((lesson1, lesson2) => lesson1.StartTime.CompareTo(lesson2.StartTime));
        }

        internal void Remove(Lesson lesson)
        {
            _lessons.Remove(lesson);
        }
    }
}