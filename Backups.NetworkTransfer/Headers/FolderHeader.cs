using System;
using System.Linq;
using System.Text;

namespace Backups.NetworkTransfer.Headers
{
    public class FolderHeader
    {
        private readonly byte[] _nameSize;
        private readonly byte[] _name;
        private readonly byte[] _count;

        public FolderHeader(string name, int filesCount)
        {
            _name = Encoding.ASCII.GetBytes(name);
            _nameSize = BitConverter.GetBytes(_name.Length);
            _count = BitConverter.GetBytes(filesCount);
        }

        public FolderHeader(byte[] bytes)
        {
            _nameSize = bytes.Take(4).ToArray();
            int nameSize = BitConverter.ToInt32(_nameSize);
            _name = bytes.Skip(4).Take(nameSize).ToArray();
            _count = bytes.Skip(4 + nameSize).Take(4).ToArray();
        }

        public byte[] GetByteHeader() => _nameSize.Concat(_name).Concat(_count).ToArray();

        public string GetFolderName() => Encoding.ASCII.GetString(_name);

        public int GetFilesCount() => BitConverter.ToInt32(_count);
    }
}