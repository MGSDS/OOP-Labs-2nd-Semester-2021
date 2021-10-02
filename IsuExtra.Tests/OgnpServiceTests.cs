using System;
using Isu.Entities;
using Isu.Services;
using Isu.Tools;
using IsuExtra.Entities;
using IsuExtra.Services;
using NUnit.Framework;

namespace IsuExtra.Tests
{
    public class OgnpServiceTests
    {
        private IsuService _isu;
        private OgnpService _ognp;
        [SetUp]
        public void Setup()
        {
            _isu = new IsuService(30);
            _ognp = new OgnpService(_isu);
            _isu.AddGroup("M3200");
            _isu.AddGroup("M3231");
            _isu.AddStudent(_isu.FindGroup("M3200")!, "Одногрупп 1");
            _isu.AddStudent(_isu.FindGroup("M3200")!, "Одногрупп 2");
            _isu.AddStudent(_isu.FindGroup("M3200")!, "Одногрупп 3");
            _isu.AddStudent(_isu.FindGroup("M3231")!, "Кт 1");
            _isu.AddStudent(_isu.FindGroup("M3231")!, "Кт 2");
            _isu.AddStudent(_isu.FindGroup("M3231")!, "Кт 3");
            _ognp.SynchronizeWithIsu();
        }

        [Test]
        public void RegisterDepartment_DepartmentRegistered()
        {
            _ognp.RegisterDepartment("ИТИП", 'M');
            Assert.IsNotEmpty(_ognp.Departments);
        }

        [Test]
        public void RegisterDepartment_DepartmentWithSuchNameAlreadyExistsExeption()
        {
            _ognp.RegisterDepartment("ИТИП", 'M');
            Assert.Catch<IsuException>(() =>
            {
                _ognp.RegisterDepartment("ИТИП", 'N');
            });
        }
        
        [Test]
        public void RegisterDepartment_DepartmentWithSuchCodeLetterAlreadyExistsExeption()
        {
            _ognp.RegisterDepartment("ИТИП", 'M');
            Assert.Catch<IsuException>(() =>
            {
                _ognp.RegisterDepartment("NONE", 'M');
            });
        }
        
        [Test]
        public void RegisterOgnp_OgnpRegistered()
        {
            _ognp.RegisterOgnp("Огнп", _ognp.RegisterDepartment("ИТИП", 'M'));
            Assert.IsNotEmpty(_ognp.Ognps);
        }
        
        [Test]
        public void RegisterOgnp_NoSuchDepartmentRegisteredException()
        {
            Assert.Catch<IsuException>(() =>
            {
                _ognp.RegisterOgnp("Огнп", new Department('M', "ИТИП"));
            });
        }
        
        [Test]
        public void RegisterOgnp_OgnpWithSuchNameRegisteredException()
        {
            Department department = _ognp.RegisterDepartment("ИТИП", 'M');
            _ognp.RegisterOgnp("Огнп", department);
            Assert.Catch<IsuException>(() =>
            {
                _ognp.RegisterOgnp("Огнп", department);
            });
        }

        [Test]
        public void CreateOgnpStream_OgnpStreamCreated()
        {
            Department department = _ognp.RegisterDepartment("ИТИП", 'M');
            Ognp ognp = _ognp.RegisterOgnp("Огнп", department);
            _ognp.CreateOgnpStream(ognp, 30);
            Assert.IsNotEmpty(_ognp.Ognps[0].Streams);
        }
        
        [Test]
        public void CreateOgnpStream_OgnpIsNotRegisterdException()
        {
            Department department = _ognp.RegisterDepartment("ИТИП", 'M');
            Ognp ognp = new Ognp("Огнп", department);
            Assert.Catch<IsuException>(() =>
            {
                _ognp.CreateOgnpStream(ognp, 30);
            });
        }

        [Test]
        public void Deduct_StudentDeducted()
        {
            Department department = _ognp.RegisterDepartment("ИТИП", 'N');
            Ognp ognp = _ognp.RegisterOgnp("Огнп", department);
            Stream stream = _ognp.CreateOgnpStream(ognp, 30);
            _ognp.Enroll(_ognp.OgnpChoises[0].Student, stream);
            _ognp.Deduct(_ognp.OgnpChoises[0].Student, stream);
            Assert.IsEmpty(_ognp.OgnpChoises[0].Streams);
            Assert.IsEmpty(ognp.Streams[0].Students);
        }

        [Test]
        public void Enroll_NoSuchStreamRegisteredExeptions()
        {
            Department department = _ognp.RegisterDepartment("ИТИП", 'N');
            Ognp ognp = _ognp.RegisterOgnp("Огнп", department);
            var stream = new Stream(30, "КТ", ognp);
            Assert.Catch<IsuException>(() =>
            {
                _ognp.Enroll(_ognp.OgnpChoises[0].Student, stream);
            });
        }
        
        [Test]
        public void Enroll_OgnpCanNotOverlapMainTimetableExeption()
        {
            Department department = _ognp.RegisterDepartment("ИТИП", 'N');
            Ognp ognp = _ognp.RegisterOgnp("Огнп", department);
            Stream stream = _ognp.CreateOgnpStream(ognp, 30);
            Mentor mentor = _ognp.CreateMentor("mentor");
            _ognp.AddLesson(stream, 0, mentor, new TimeOnly(10,00), new TimeOnly(11, 40), 404);
            _ognp.AddLesson(_ognp.GroupTimetables[0], 0, mentor, new TimeOnly(10,00), new TimeOnly(11, 40), 404);
            Assert.Catch<IsuException>(() =>
            {
                _ognp.Enroll(_ognp.OgnpChoises[0].Student, stream);
            });
        }
        
        [Test]
        public void Enroll_TheStudentIsAlreadyStudying2OgnpsException()
        {
            Department department = _ognp.RegisterDepartment("ИТИП", 'N');
            Ognp ognp = _ognp.RegisterOgnp("Огнп", department);
            Ognp ognp2 = _ognp.RegisterOgnp("Огнп2", department);
            Ognp ognp3 = _ognp.RegisterOgnp("Огнп3", department);
            Stream stream = _ognp.CreateOgnpStream(ognp, 30);
            Stream stream2 = _ognp.CreateOgnpStream(ognp2, 30);
            Stream stream3 = _ognp.CreateOgnpStream(ognp3, 30);
            Student student = _ognp.OgnpChoises[0].Student;
            _ognp.Enroll(student, stream);
            _ognp.Enroll(student, stream2);
            Assert.Catch<IsuException>(() =>
            {
                _ognp.Enroll(student, stream3);
            });
        }
        
        [Test]
        public void Enroll_BothOgnpsMustBeFromTheSameFacultyException()
        {
            Department department = _ognp.RegisterDepartment("ИТИП", 'N');
            Department department2 = _ognp.RegisterDepartment("ИТИП2", 'S');
            Ognp ognp = _ognp.RegisterOgnp("Огнп", department);
            Ognp ognp2 = _ognp.RegisterOgnp("Огнп2", department2);
            Stream stream = _ognp.CreateOgnpStream(ognp, 30);
            Stream stream2 = _ognp.CreateOgnpStream(ognp2, 30);
            _ognp.Enroll(_ognp.OgnpChoises[0].Student, stream);
            Assert.Catch<IsuException>(() =>
            {
                _ognp.Enroll(_ognp.OgnpChoises[0].Student, stream2);
            });
        }
        
        [Test]
        public void Enroll_StudentAlreadyEnrolledException()
        {
            Department department = _ognp.RegisterDepartment("ИТИП", 'N');
            Ognp ognp = _ognp.RegisterOgnp("Огнп", department);
            Stream stream = _ognp.CreateOgnpStream(ognp, 30);
            _ognp.Enroll(_ognp.OgnpChoises[0].Student, stream);
            Assert.Catch<IsuException>(() =>
            {
                _ognp.Enroll(_ognp.OgnpChoises[0].Student, stream);
            });
        }
        
        [Test]
        public void Enroll_StudentsLimitHasBeenReachedInOgnpException()
        {
            Department department = _ognp.RegisterDepartment("ИТИП", 'N');
            Ognp ognp = _ognp.RegisterOgnp("Огнп", department);
            Stream stream = _ognp.CreateOgnpStream(ognp, 0);
            Assert.Catch<IsuException>(() =>
            {
                _ognp.Enroll(_ognp.OgnpChoises[0].Student, stream);
            });
        }
        
        [Test]
        public void Enroll_StudentEnrolled()
        {
            Department department = _ognp.RegisterDepartment("ИТИП", 'N');
            Ognp ognp = _ognp.RegisterOgnp("Огнп", department);
            Stream stream = _ognp.CreateOgnpStream(ognp, 30);
            _ognp.Enroll(_ognp.OgnpChoises[0].Student, stream);
            Assert.AreEqual(_ognp.OgnpChoises[0].Streams[0], stream);
            Assert.AreEqual(_ognp.Ognps[0].Streams[0].Students[0], _ognp.OgnpChoises[0].Student);
        }

        [Test]
        public void Deduct_NoSuchStreamRegisteredExeptions()
        {
            Department department = _ognp.RegisterDepartment("ИТИП", 'N');
            Ognp ognp = _ognp.RegisterOgnp("Огнп", department);
            var stream = new Stream(30, "КТ", ognp);
            Assert.Catch<IsuException>(() =>
            {
                _ognp.Deduct(_ognp.OgnpChoises[0].Student, stream);
            });
        }
        
        [Test]
        public void GetStudents_NoSuchStreamRegisteredExeptions()
        {
            Department department = _ognp.RegisterDepartment("ИТИП", 'N');
            Ognp ognp = _ognp.RegisterOgnp("Огнп", department);
            var stream = new Stream(30, "КТ", ognp);
            Assert.Catch<IsuException>(() =>
            {
                _ognp.GetStudents(stream);
            });
        }
        
        [Test]
        public void GetStudents_StudentsReturned()
        {
            Department department = _ognp.RegisterDepartment("ИТИП", 'N');
            Ognp ognp = _ognp.RegisterOgnp("Огнп", department);
            Stream stream = _ognp.CreateOgnpStream(ognp, 30);
            _ognp.Enroll(_ognp.GroupTimetables[0].Group.Students[0], stream);
            _ognp.Enroll(_ognp.GroupTimetables[0].Group.Students[1], stream);
            _ognp.Enroll(_ognp.GroupTimetables[0].Group.Students[2], stream);
            _ognp.Enroll(_ognp.GroupTimetables[1].Group.Students[0], stream);
            _ognp.Enroll(_ognp.GroupTimetables[1].Group.Students[1], stream);
            Assert.AreEqual(5, _ognp.GetStudents(stream).Count);
        }
        
        [Test]
        public void GetNotEnrolledStudents_StudentsReturned()
        {
            Department department = _ognp.RegisterDepartment("ИТИП", 'N');
            Ognp ognp = _ognp.RegisterOgnp("Огнп", department);
            Ognp ognp2 = _ognp.RegisterOgnp("Огнп2", department);
            Stream stream = _ognp.CreateOgnpStream(ognp, 30);
            Stream stream2 = _ognp.CreateOgnpStream(ognp2, 30);
            _ognp.Enroll(_ognp.GroupTimetables[0].Group.Students[0], stream);
            _ognp.Enroll(_ognp.GroupTimetables[0].Group.Students[1], stream);
            _ognp.Enroll(_ognp.GroupTimetables[0].Group.Students[0], stream2);
            _ognp.Enroll(_ognp.GroupTimetables[0].Group.Students[1], stream2);
            Assert.AreEqual(_ognp.GroupTimetables[0].Group.Students.Count - 2,
                _ognp.GetNotEnrolledStudents(_ognp.GroupTimetables[0].Group).Count);
        }
        
        [Test]
        public void AddLesson_LessonAdded()
        {
            Department department = _ognp.RegisterDepartment("ИТИП", 'N');
            Ognp ognp = _ognp.RegisterOgnp("Огнп", department);
            Stream stream = _ognp.CreateOgnpStream(ognp, 30);
            _ognp.AddLesson(stream, 0, _ognp.CreateMentor("Mentor"), new TimeOnly(10, 00), new TimeOnly(11, 40), 404);
            Assert.IsNotEmpty(stream.Timetable.Days[0].Lessons);
        }
        
        [Test]
        public void AddLesson_StreamIsNotRegisteredException()
        {
            Department department = _ognp.RegisterDepartment("ИТИП", 'N');
            Ognp ognp = _ognp.RegisterOgnp("Огнп", department);
            var stream = new Stream(30, "ongp", ognp);
            Assert.Catch<IsuException>(() =>
            {
                _ognp.AddLesson(stream, 0, _ognp.CreateMentor("Mentor"), new TimeOnly(10, 00), new TimeOnly(11, 40), 404);
            });
        }
        
        [Test]
        public void AddLesson_MentorIsNotRegistered()
        {
            Department department = _ognp.RegisterDepartment("ИТИП", 'N');
            Ognp ognp = _ognp.RegisterOgnp("Огнп", department);
            Stream stream = _ognp.CreateOgnpStream(ognp, 30);
            Assert.Catch<IsuException>(() =>
            {
                _ognp.AddLesson(stream, 0, new Mentor(0,"Mentor"), new TimeOnly(10, 00), new TimeOnly(11, 40), 404);
            });
        }
    }
}