using Isu.Entities;
using Isu.Tools;

namespace Isu.DataTypes
{
    public class CourseNumber
    {
        public CourseNumber(int courseNumber)
        {
            if (courseNumber > 4 || courseNumber < 1)
            {
                throw new IsuException("Invalid CourseNum");
            }

            Course = courseNumber;
        }

        public CourseNumber(string groupName)
            : this(int.Parse(groupName.Substring(2, 1))) { }

        public int Course { get; private set; }

        public void Increase()
        {
            if (Course == 4)
            {
                throw new IsuException("Already last course");
            }

            Course++;
        }

        public override bool Equals(object obj) => this.Equals(obj as CourseNumber);

        public override int GetHashCode()
        {
            return (int)Course;
        }

        public bool Equals(CourseNumber obj)
        {
            return Course == obj.Course;
        }
    }
}