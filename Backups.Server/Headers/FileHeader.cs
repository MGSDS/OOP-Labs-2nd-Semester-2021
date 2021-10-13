using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Backups.Server.Headers
{
    public class FileHeader
    {
        private byte[] _size;
        private byte[] _nameSize;
        private byte[] _name;

        public FileHeader(string name, int size)
        {
            _name = Encoding.ASCII.GetBytes(name);
            int nameSize = _name.Length;
            _size = BitConverter.GetBytes(size);
            _nameSize = BitConverter.GetBytes(nameSize);
        }

        public FileHeader(byte[] bytes)
        {
            _nameSize = bytes.Take(4).ToArray();
            int nameSize = BitConverter.ToInt32(_nameSize);
            _name = bytes.Skip(4).Take(nameSize).ToArray();
            _size = bytes.Skip(4 + nameSize).ToArray();
        }

        public byte[] GetByteHeader()
        {
            return _nameSize.Concat(_name).Concat(_size).ToArray();
        }

        public string GetName()
        {
            return Encoding.ASCII.GetString(_name);
        }

        public int GetSize()
        {
            return BitConverter.ToInt32(_size);
        }
    }
}