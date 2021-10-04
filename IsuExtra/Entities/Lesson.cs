using System;
using Isu.Entities;
using Isu.Tools;

namespace IsuExtra.Entities
{
    public class Lesson : IComparable<Lesson>
    {
        public Lesson(Group group, Mentor mentor, TimeOnly startTime, TimeOnly endTime, uint room)
            : this(mentor, startTime, endTime, room)
        {
            Group = group;
            Stream = null;
        }

        public Lesson(Stream stream, Mentor mentor, TimeOnly startTime, TimeOnly endTime, uint room)
            : this(mentor, startTime, endTime, room)
        {
            Group = null;
            Stream = stream;
        }

        private Lesson(Mentor mentor, TimeOnly startTime, TimeOnly endTime, uint room)
        {
            if (startTime >= endTime)
                throw new IsuException("EndTime can not be earlier than StartTime");
            Mentor = mentor;
            StartTime = startTime;
            EndTime = endTime;
            Room = room;
        }

        public Mentor Mentor { get; }
        public TimeOnly StartTime { get; }
        public TimeOnly EndTime { get; }
        public Group Group { get; }

        public Stream Stream { get; }
        public uint Room { get; }

        public bool CheckIntersection(Lesson lesson)
        {
            return lesson.StartTime.IsBetween(StartTime, EndTime) ||
                   lesson.EndTime.IsBetween(StartTime, EndTime);
        }

        public int CompareTo(Lesson other)
        {
            return StartTime.CompareTo(other.StartTime);
        }
    }
}