using System;
using System.Collections.Generic;
using System.Linq;
using Isu.Entities;
using Isu.Services;
using Isu.Tools;
using IsuExtra.Entities;

namespace IsuExtra.Services
{
    public class OgnpService
    {
        private List<OgnpChoise> _ognpChoises;
        private List<Department> _departments;
        private List<GroupTimetable> _groupTimetables;
        private List<Ognp> _ognps;
        private List<Mentor> _mentors;

        public OgnpService(IsuService isu)
        {
            Isu = isu;
            _ognps = new List<Ognp>();
            _departments = new List<Department>();
            _groupTimetables = isu.Groups.Select(x => new GroupTimetable(x, new Timetable())).ToList();
            _ognpChoises = isu.Students.Select(x => new OgnpChoise(x)).ToList();
            _mentors = new List<Mentor>();
        }

        public IsuService Isu { get; }
        public IReadOnlyList<OgnpChoise> OgnpChoises => _ognpChoises;
        public IReadOnlyList<Department> Departments => _departments;
        public IReadOnlyList<GroupTimetable> GroupTimetables => _groupTimetables;
        public IReadOnlyList<Ognp> Ognps => _ognps;
        public IReadOnlyList<Mentor> Mentors => _mentors;

        public void SynchronizeWithIsu()
        {
            foreach (Group @group in Isu.Groups.Where(@group => !_groupTimetables.Any(x => x.Group.Equals(@group))))
                _groupTimetables.Add(new GroupTimetable(@group, new Timetable()));

            foreach (Student @student in Isu.Students.Where(@student =>
                !_ognpChoises.Any(x => x.Student.Equals(@student))))
                _ognpChoises.Add(new OgnpChoise(student));
        }

        public Department RegisterDepartment(string name, char codeLetter)
        {
            if (_departments.Find(department => department.Name == name) is not null)
                throw new IsuException("Department with same name already exists");
            if (_departments.Find(department => department.CodeLetter == codeLetter) is not null)
                throw new IsuException("Department with same code letter already exists");
            var newDepartment = new Department(codeLetter, name);
            _departments.Add(newDepartment);
            return newDepartment;
        }

        public Ognp RegisterOgnp(string name, Department department)
        {
            if (!_departments.Contains(department))
                throw new IsuException("No such department registered");
            if (_ognps.Any(x => x.Name == name))
                throw new IsuException("Ognp with such name already registered");
            Ognp ognp = department.AddOgnp(name);
            _ognps.Add(ognp);
            return ognp;
        }

        public Stream CreateOgnpStream(Ognp ognp, ushort maxStudentsCount)
        {
            if (!_ognps.Contains(ognp))
                throw new IsuException("Ognp is not registered");
            return ognp.AddStream(maxStudentsCount);
        }

        public void Enroll(Student student, Stream stream)
        {
            Ognp ognp = _ognps.Find(x => x.Equals(stream.Ognp)) ??
                        throw new IsuException("Ognp is not registered");
            if (!ognp.Streams.Contains(stream))
                throw new IsuException("No such stream registered");
            OgnpChoise ognpChoise = _ognpChoises.Find(x => x.Student.Equals(student)) ??
                                    throw new IsuException("No such student registered");
            if (stream.Timetable.CheckIntersection(_groupTimetables.Find(x => x.Group.Equals(student.StudyGroup))))
                throw new IsuException("Ognp timetable can not overlap main timetable");
            ognpChoise.Enroll(stream);
        }

        public void Deduct(Student student, Stream stream)
        {
            Ognp ognp = _ognps.Find(x => x.Equals(stream.Ognp)) ??
                        throw new IsuException("Ognp is not registered");
            if (!ognp.Streams.Contains(stream))
                throw new IsuException("No such stream registered");
            OgnpChoise ognpChoise = _ognpChoises.Find(x => x.Student.Equals(student)) ??
                                    throw new IsuException("No such student registered");
            ognpChoise.Deduct(stream);
        }

        public IReadOnlyList<Student> GetStudents(Stream stream)
        {
            Ognp ognp = _ognps.Find(x => x.Equals(stream.Ognp)) ??
                        throw new IsuException("Ognp is not registered");
            if (!ognp.Streams.Contains(stream))
                throw new IsuException("No such stream registered");
            return stream.Students;
        }

        public IReadOnlyList<Student> GetNotEnrolledStudents(Group group)
        {
            return (from student in @group.Students
                select _ognpChoises.Find(x => x.Student.Equals(student))
                into ognpChoise
                where ognpChoise is null || ognpChoise.Streams.Count != 2
                select ognpChoise.Student).ToList();
        }

        public void AddLesson(GroupTimetable groupTimetable, ushort day, Mentor mentor, ushort startTime, ushort endTime, uint room)
        {
            if (!_groupTimetables.Contains(groupTimetable))
                throw new IsuException("GroupTimetable is not registered");
            if (!_mentors.Contains(mentor))
                throw new IsuException("Mentor is not registered");
            groupTimetable.AddLesson(day, mentor, startTime, endTime, room);
        }

        public void AddLesson(Stream stream, ushort day, Mentor mentor, ushort startTime, ushort endTime, uint room)
        {
            if (_ognps.All(x => !x.Streams.Contains(stream)))
                throw new IsuException("stream is not registered");
            if (!_mentors.Contains(mentor))
                throw new IsuException("Mentor is not registered");
            stream.AddLesson(day, mentor, startTime, endTime, room);
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

        public OgnpChoise FindOgnpChoise(uint studentId)
        {
            return _ognpChoises.Find(x => x.Student.Id == studentId);
        }

        public Ognp FindOgnp(string ognpName)
        {
            return _ognps.Find(x => x.Name == ognpName);
        }

        public Department FindDepartment(string departmentName)
        {
            return _departments.Find(x => x.Name == departmentName);
        }

        public Department FindDepartment(char codeLetter)
        {
            return _departments.Find(x => x.CodeLetter == codeLetter);
        }

        public Mentor FindMentor(string name)
        {
            return _mentors.Find(x => x.Name == name);
        }
    }
}