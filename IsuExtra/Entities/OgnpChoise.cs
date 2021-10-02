using System.Collections.Generic;
using Isu.Entities;
using Isu.Tools;

namespace IsuExtra.Entities
{
    public class OgnpChoise
    {
        private List<Stream> _streams;

        public OgnpChoise(Student student)
        {
            _streams = new List<Stream>();
            Student = student;
        }

        public Student Student { get; }
        public IReadOnlyList<Stream> Streams => _streams;

        internal void Enroll(Stream stream)
        {
            if (_streams.Count >= 2)
                throw new IsuException("The student is already studying 2 ognps");
            if (_streams.Count == 1 && !_streams[0].Ognp.Department.Equals(stream.Ognp.Department))
                throw new IsuException("Both ognps must be from the same faculty");
            if (Student.StudyGroup.Name[0] == stream.Ognp.Department.CodeLetter)
                throw new IsuException("Unable to enroll native department course");
            stream.Enroll(Student);
            _streams.Add(stream);
        }

        internal void Deduct(Stream stream)
        {
            stream.Deduct(Student);
            _streams.Remove(stream);
        }
    }
}