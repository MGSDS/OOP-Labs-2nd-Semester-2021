using System;
using System.Collections.Generic;
using System.Linq;
using Isu.Services;
using Isu.Tools;
using IsuExtra.Entities;
using Group = Isu.Entities.Group;

namespace IsuExtra.Services
{
    public class TimetableService
    {
        private List<GroupTimetable> _groupTimetables;
        private List<Mentor> _mentors;

        public TimetableService(IsuService isu)
        {
            Isu = isu;
            _groupTimetables = isu.Groups.Select(x => new GroupTimetable(x, new Timetable())).ToList();
            _mentors = new List<Mentor>();
        }

        public IReadOnlyList<GroupTimetable> GroupTimetables => _groupTimetables;
        public IReadOnlyList<Mentor> Mentors => _mentors;
        public IsuService Isu { get; }

        public void SynchronizeWithIsu()
        {
            foreach (Group group in Isu.Groups.Where(group => !_groupTimetables.Any(x => x.Group.Equals(group))))
                _groupTimetables.Add(new GroupTimetable(group, new Timetable()));
        }

        public Mentor CreateMentor(string name)
        {
            Mentor mentor = new Mentor(_mentors.Count, name);
            _mentors.Add(mentor);
            return mentor;
        }

        public GroupTimetable FindGroupTimetable(string groupName)
        {
            return _groupTimetables.Find(x => x.Group.Name == groupName);
        }

        public void AddLesson(GroupTimetable groupTimetable, ushort day, Mentor mentor, TimeOnly startTime, TimeOnly endTime, uint room)
        {
            GetRegisteredGroupTimetable(groupTimetable).AddLesson(day, GetRegisteredMentor(mentor), startTime, endTime, room);
        }

        public Mentor FindMentor(string name)
        {
            return _mentors.Find(x => x.Name == name);
        }

        internal Mentor GetRegisteredMentor(Mentor mentor)
        {
            return _mentors.Find(x => x.Id == mentor.Id) ??
                   throw new IsuException("Mentor is not registered");
        }

        private GroupTimetable GetRegisteredGroupTimetable(GroupTimetable groupTimetable)
        {
            return _groupTimetables.Find(x => x.Group.Equals(groupTimetable.Group)) ??
                   throw new IsuException("GroupTimetable is not registered");
        }
    }
}