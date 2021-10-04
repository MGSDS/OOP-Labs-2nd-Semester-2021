using System;
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
            _lesson = new Lesson(new Group("M3200", 32), new Mentor(0, "q"), new TimeOnly(10, 00), new TimeOnly(11, 40), 403);
        }

        [Test]
        [TestCase(10, 00, 11, 30)]
        [TestCase(11, 00, 12, 30)]
        [TestCase(9, 00, 10, 30)]
        public void CheckIntersection_IntersectionFound(int startTimeHours, int startTimeMinutes,  int endTimeHours, int endTimeMinutes)
        {
            TimeOnly startTime = new TimeOnly(startTimeHours, startTimeMinutes);
            TimeOnly endTime = new TimeOnly(endTimeHours, endTimeMinutes);
            Assert.True(_lesson.CheckIntersection(new Lesson(new Group("M3200", 32), new Mentor(0, "q"), startTime, endTime, 403)));
        }
        
        [Test]
        [TestCase(11, 40, 13, 10)]
        [TestCase(8, 20, 9, 50)]
        public void CheckIntersection_IntersectionNotFound(int startTimeHours, int startTimeMinutes,  int endTimeHours, int endTimeMinutes)
        {
            TimeOnly startTime = new TimeOnly(startTimeHours, startTimeMinutes);
            TimeOnly endTime = new TimeOnly(endTimeHours, endTimeMinutes);
            Assert.False(_lesson.CheckIntersection(new Lesson(new Group("M3200", 32), new Mentor(0, "q"), startTime, endTime, 403)));
        }
    }
}