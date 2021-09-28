using System.Collections.Generic;

namespace IsuExtra.Entities
{
    public class Ognp
    {
        private List<Stream> _streams;
        public Ognp(string name, Department department)
        {
            Name = name;
            Department = department;
            _streams = new List<Stream>();
        }

        public IReadOnlyList<Stream> Streams => _streams;
        public string Name { get; }
        public Department Department { get; }

        public Stream AddStream(ushort maxStudentsCount)
        {
            Stream stream = new Stream(maxStudentsCount, $"{Name}{_streams.Count}", this);
            _streams.Add(stream);
            return stream;
        }
    }
}