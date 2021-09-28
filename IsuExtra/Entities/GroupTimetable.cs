using Isu.Entities;
using Isu.Tools;

namespace IsuExtra.Entities
{
    public class GroupTimetable
    {
        public GroupTimetable(Group group, Timetable timetable)
        {
            Group = group;
            Timetable = timetable;
        }

        public Group Group { get; }
        public Timetable Timetable { get; }

        internal void AddLesson(ushort day, Mentor mentor, ushort startTime, ushort endTime, uint room)
        {
            var lesson = new Lesson(Group, mentor, startTime, endTime, room);
            if (day >= 7)
                throw new IsuException("Day should be in range 0..6");
            Timetable.Days[day].Add(new Lesson(Group, mentor, startTime, endTime, room));
        }
    }
}