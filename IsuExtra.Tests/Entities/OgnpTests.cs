using IsuExtra.Entities;
using NUnit.Framework;

namespace IsuExtra.Tests.Entities
{
    public class OgnpTests
    {
        private Ognp _ognp;
        [SetUp]
        public void SetUp()
        {
            _ognp = new Ognp("КТ", new Department('M', "ИТИП"));
        }

        [Test]
        public void AddStream_StreamAdded()
        {
            _ognp.AddStream(10);
            Assert.IsNotEmpty(_ognp.Streams);
        }
    }
}