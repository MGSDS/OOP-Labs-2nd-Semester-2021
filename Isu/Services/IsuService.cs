using System.Collections.Generic;
using System.Linq;
using Isu.DataTypes;
using Isu.Entities;
using Isu.Tools;

#nullable enable

namespace Isu.Services
{
    public class IsuService : IIsuService
    {
        private uint _maxGroupCapacity;
        public IsuService(uint maxGroupCapacity)
        {
            _maxGroupCapacity = maxGroupCapacity;
            Groups = new List<Group>();
            Students = new List<Student>();
            LastId = 0;
        }

        public List<Group> Groups { get; private set; }
        public List<Student> Students { get; private set; }

        private ulong LastId { get; set; }

        public Group AddGroup(string name)
        {
            Group newGroup = new Group(name, _maxGroupCapacity);
            Groups.Add(newGroup);
            return newGroup;
        }

        public Student AddStudent(Group @group, string name)
        {
            Student newStudent = new Student(name, LastId, group);
            Students.Add(newStudent);
            LastId += 1;
            return newStudent;
        }

        public Student GetStudent(int id)
        {
            Student? found = Students.Find(st => st.Id == (ulong)id);
            if (found == null)
                throw new IsuException("There is no students with such id");
            return found;
        }

        public Student? FindStudent(string name)
        {
            Student? found = Students.FirstOrDefault(st => st.Name == name);
            return found;
        }

        public List<Student>? FindStudents(string groupName)
        {
            List<Student>? students = FindGroup(groupName)?.Students;
            return students;
        }

        public List<Student> FindStudents(CourseNumber courseNumber)
        {
            return Students.FindAll(st => st.StudyGroup.Course.Equals(courseNumber));
        }

        public Group? FindGroup(string groupName)
        {
            Group? group = Groups.FirstOrDefault(gr => gr.Name == groupName);
            return group;
        }

        public List<Group> FindGroups(CourseNumber courseNumber)
        {
            return Groups.FindAll(gr => gr.Course.Equals(courseNumber));
        }

        public void ChangeStudentGroup(Student student, Group newGroup)
        {
            student.StudyGroup?.RemoveStudent(student.Id);
            newGroup.AddStudent(student);
        }
    }
}