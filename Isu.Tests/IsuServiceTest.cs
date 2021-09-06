using Isu.Services;
using Isu.Tools;
using NUnit.Framework;

namespace Isu.Tests
{
    public class Tests
    {
        private IIsuService _isuService;

        [SetUp]
        public void Setup()
        {
            _isuService = new IsuService(5);
            _isuService.AddGroup("M3200");
            _isuService.AddGroup("M3201");
            _isuService.AddGroup("M3104");
        }

        [Test]
        public void AddStudentToGroup_StudentHasGroupAndGroupContainsStudent()
        {
            _isuService.AddStudent(_isuService.FindGroup("M3104"), "pervak 1");
            Assert.AreEqual(_isuService.FindGroup("M3104"), _isuService.FindStudent("pervak 1").StudyGroup);
            Assert.AreEqual(_isuService.FindGroup("M3104").Students.Contains(_isuService.FindStudent("pervak 1")), true);
        }

        [Test]
        public void ReachMaxStudentPerGroup_ThrowException()
        {
            Assert.Catch<IsuException>(() =>
            {
                _isuService.AddStudent(_isuService.FindGroup("M3104"), "pervak 1");
                _isuService.AddStudent(_isuService.FindGroup("M3104"), "pervak 2");
                _isuService.AddStudent(_isuService.FindGroup("M3104"), "pervak 3");
                _isuService.AddStudent(_isuService.FindGroup("M3104"), "pervak 4");
                _isuService.AddStudent(_isuService.FindGroup("M3104"), "pervak 5");
                _isuService.AddStudent(_isuService.FindGroup("M3104"), "pervak 6");
            });
        }

        [Test]
        public void CreateGroupWithInvalidName_ThrowException()
        {
            Assert.Catch<IsuException>(() =>
            {
                _isuService.AddGroup("M3800");
            });
        }

        [Test]
        public void TransferStudentToAnotherGroup_GroupChanged()
        {
            _isuService.AddStudent(_isuService.FindGroup("M3104"), "pervak 1");
            _isuService.ChangeStudentGroup(_isuService.FindStudent("pervak 1"),_isuService.FindGroup("M3200"));
            Assert.AreEqual(_isuService.FindStudent("pervak 1").StudyGroup, _isuService.FindGroup("M3200"));
        }
    }
}