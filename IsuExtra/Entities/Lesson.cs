using Isu.Entities;
using Isu.Tools;

namespace IsuExtra.Entities
{
    public class Lesson
    {
        public Lesson(Group group, Mentor mentor, uint startTime, uint endTime, uint room)
        {
            if (startTime >= endTime)
                throw new IsuException("EndTime can not be earlier than StartTime");
            Group = group;
            Stream = null;
            Mentor = mentor;
            StartTime = startTime;
            EndTime = endTime;
            Room = room;
        }

        public Lesson(Stream stream, Mentor mentor, uint startTime, uint endTime, uint room)
        {
            if (startTime >= endTime)
                throw new IsuException("EndTime can not be earlier than StartTime");
            Group = null;
            Stream = stream;
            Mentor = mentor;
            StartTime = startTime;
            EndTime = endTime;
            Room = room;
        }

        public Mentor Mentor { get; }
        public uint StartTime { get; }
        public uint EndTime { get; }
        public Group Group { get; }

        public Stream Stream { get; }
        public uint Room { get; }

        public bool CheckIntersection(Lesson lesson)
        {
            return (StartTime <= lesson.StartTime && lesson.StartTime <= EndTime) ||
                   (StartTime <= lesson.EndTime && lesson.EndTime <= EndTime);
        }
    }
}