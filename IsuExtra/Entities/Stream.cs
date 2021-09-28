using System;
using System.Collections.Generic;
using Isu.Entities;
using Isu.Tools;

namespace IsuExtra.Entities
{
    public class Stream
    {
        private List<Student> _students;
        public Stream(ushort maxStudentsCount, string name, Ognp ognp)
        {
            MaxStudentsCount = maxStudentsCount;
            Name = name;
            Ognp = ognp;
            _students = new List<Student>(maxStudentsCount);
            Timetable = new Timetable();
        }

        public string Name { get; }
        public Ognp Ognp { get; }
        public IReadOnlyList<Student> Students => _students;
        public ushort MaxStudentsCount { get; }

        public Timetable Timetable { get; }

        internal void AddLesson(ushort day, Mentor mentor, ushort startTime, ushort endTime, uint room)
        {
            var lesson = new Lesson(this, mentor, startTime, endTime, room);
            if (day >= 7)
                throw new IsuException("Day should be in range 0..6");
            Timetable.Days[day].Add(lesson);
        }

        internal void Enroll(Student student)
        {
            if (_students.Count == MaxStudentsCount)
                throw new IsuException("Students limit has been reached in ognp");
            if (_students.Contains(student))
                throw new IsuException("Student already enrolled");
            _students.Add(student);
        }

        internal void Deduct(Student student)
        {
            _students.Remove(student);
        }
    }
}