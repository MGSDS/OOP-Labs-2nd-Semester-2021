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
        private List<Ognp> _ognps;

        public OgnpService(IsuService isu, TimetableService timetableService)
        {
            Isu = isu;
            TimetableService = timetableService;
            _ognps = new List<Ognp>();
            _departments = new List<Department>();
            _ognpChoises = isu.Students.Select(x => new OgnpChoise(x)).ToList();
        }

        public IsuService Isu { get; }
        public TimetableService TimetableService { get; }
        public IReadOnlyList<OgnpChoise> OgnpChoises => _ognpChoises;
        public IReadOnlyList<Department> Departments => _departments;
        public IReadOnlyList<Ognp> Ognps => _ognps;

        public void SynchronizeWithIsu()
        {
            TimetableService.SynchronizeWithIsu();
            foreach (Student student in Isu.Students.Where(student =>
                !_ognpChoises.Any(x => x.Student.Equals(student))))
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
            if (_ognps.Any(x => x.Name == name))
                throw new IsuException("Ognp with such name already registered");
            Ognp ognp = GetRegisteredDepartment(department).AddOgnp(name);
            _ognps.Add(ognp);
            return ognp;
        }

        public Stream CreateOgnpStream(Ognp ognp, ushort maxStudentsCount)
        {
            return GetRegisteredOgnp(ognp).AddStream(maxStudentsCount);
        }

        public void Enroll(Student student, Stream stream)
        {
            Stream foundStream = GetRegisteredStream(stream);
            OgnpChoise ognpChoise = GetRegisteredOgnpChoise(student);
            if (foundStream.Timetable.CheckIntersection(TimetableService.GroupTimetables.FirstOrDefault(x => x.Group.Equals(student.StudyGroup))))
                throw new IsuException("Ognp timetable can not overlap main timetable");
            ognpChoise.Enroll(foundStream);
        }

        public void Deduct(Student student, Stream stream)
        {
            Stream foundStream = GetRegisteredStream(stream);
            OgnpChoise ognpChoise = GetRegisteredOgnpChoise(student);
            ognpChoise.Deduct(foundStream);
        }

        public IReadOnlyList<Student> GetStudents(Stream stream)
        {
            return GetRegisteredStream(stream).Students;
        }

        public IReadOnlyList<Student> GetNotEnrolledStudents(Group group) =>
            _ognpChoises.Where(x => group.Students.Any(student => x.Student.Equals(student)))
                .Where(ognpChoise => ognpChoise is null || ognpChoise.Streams.Count != 2).Select(x => x.Student)
                .ToList();

        public void AddLesson(Stream stream, ushort day, Mentor mentor, TimeOnly startTime, TimeOnly endTime, uint room)
        {
            Stream foundStream = GetRegisteredStream(stream);
            foundStream.AddLesson(day, TimetableService.GetRegisteredMentor(mentor), startTime, endTime, room);
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

        private Department GetRegisteredDepartment(Department department)
        {
            return _departments.Find(dep => dep.Name == department.Name) ??
                               throw new IsuException("No such department registered");
        }

        private Ognp GetRegisteredOgnp(Ognp ognp)
        {
            return _ognps.Find(x => x.Name == ognp.Name) ??
                   throw new IsuException("No such ognp registered");
        }

        private Stream GetRegisteredStream(Stream stream)
        {
            return GetRegisteredOgnp(stream.Ognp).Streams.FirstOrDefault(x => x.Name == stream.Name) ??
                   throw new IsuException("No such steam registered");
        }

        private OgnpChoise GetRegisteredOgnpChoise(Student student)
        {
            return _ognpChoises.Find(x => x.Student.Equals(student)) ??
                                    throw new IsuException("No such student registered");
        }
    }
}