using Backups.Repositories;
using BackupsExtra.ClearAlgorithms;
using BackupsExtra.CompressionAlgorithms;
using BackupsExtra.CreationalAlgorithms;
using BackupsExtra.Repository;
using Moq;
using NUnit.Framework;

namespace BackupsExtra.Tests
{
    public class ClearAlgorithmsTests
    {
        private IRestorePointManageAlgorithm _restorePointCreationalAlgorithm;
        private IExtraCompressor _compressor;
        private IClearAlgorithm _clearAlgorithm;
        private Mock<ISourceRepository> _repository;
        private Mock<IExtraBackupDestinationRepository> _backupDestinationRepository;
        
        [SetUp]
        public void Setup()
        {
            
        }    
    }
}