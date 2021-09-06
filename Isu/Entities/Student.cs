#nullable enable
namespace Isu.Entities
{
    public class Student
    {
        public Student(string name, ulong id, Group studyGroup)
        {
            Id = id;
            Name = name;
            StudyGroup = studyGroup;
            studyGroup.AddStudent(this);
        }

        public ulong Id { get; }
        public string Name { get; }
        public Group StudyGroup { get; internal set; }
    }
}