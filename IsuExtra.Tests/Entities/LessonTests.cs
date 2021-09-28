using Isu.Entities;
using Isu.Services;
using IsuExtra.Entities;
using IsuExtra.Services;
using NUnit.Framework;

namespace IsuExtra.Tests.Entities
{
    public class LessonTests
    {
        private Lesson _lesson;
        [SetUp]
        public void Setup()
        {
            _lesson = new Lesson(new Group("M3200", 32), new Mentor(0, "q"), 60, 120, 403);
        }

        [Test]
        public void CheckIntersection_IntersectionFound()
        {
            Assert.True(_lesson.CheckIntersection(new Lesson(new Group("M3200", 32), new Mentor(0, "q"), 70, 130, 403)));
            Assert.True(_lesson.CheckIntersection(new Lesson(new Group("M3200", 32), new Mentor(0, "q"), 50, 110, 403)));
        }
        
        [Test]
        public void CheckIntersection_IntersectionNotFound()
        {
            Assert.False(_lesson.CheckIntersection(new Lesson(new Group("M3200", 32), new Mentor(0, "q"), 130, 210, 403)));
        }
    }
}