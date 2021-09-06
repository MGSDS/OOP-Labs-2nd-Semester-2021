using System;
using System.Collections.Generic;
using System.Linq;
using Isu.DataTypes;
using Isu.Tools;
#nullable enable

namespace Isu.Entities
{
    public class Group
    {
        private readonly uint _maxCapacity;

        public Group(string name, uint maxCapacity)
        {
            if (name.Substring(0, 2) != "M3")
            {
                throw new IsuException("Invalid group name");
            }

            _maxCapacity = maxCapacity;
            Course = new CourseNumber(name);
            Name = name;
            Students = new List<Student>();
        }

        public string Name { get; }
        public List<Student> Students { get; }
        public CourseNumber Course { get; }

        public void RemoveStudent(ulong studentId)
        {
            Student? found = Students.Find(st => st.Id == studentId);
            if (found != null)
                Students.Remove(found);
        }

        public void AddStudent(Student student)
        {
            Student? found = Students.Find(st => st.Id == student.Id);
            if (found != null)
                throw new IsuException("Student with same id already exists in this group");
            if (Students.Count >= _maxCapacity)
                throw new IsuException("Student exceeded the limit in the group");
            student.StudyGroup.RemoveStudent(student.Id);
            student.StudyGroup = this;
            Students.Add(student);
        }
    }
}