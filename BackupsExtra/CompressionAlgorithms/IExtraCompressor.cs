using System.IO;
using Backups.CompressionAlgorithms;

namespace BackupsExtra.CompressionAlgorithms
{
    public interface IExtraCompressor : ICompressor
    {
        public void Merge(Stream destination, Stream source);
    }
}