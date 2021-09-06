using Isu.Tools;

namespace Isu.DataTypes
{
    public class CourseNumber
    {
        public CourseNumber(int courseNum)
        {
            if (courseNum is > 4 or < 1)
            {
                throw new IsuException("Invalid CourseNum");
            }

            Course = courseNum;
        }

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