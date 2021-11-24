using System;
using System.Collections.Generic;
using System.IO;
using Backups.CompressionAlgorithms;
using Backups.Entities;
using Backups.Repositories;
using BackupsExtra.ClearAlgorithms;
using BackupsExtra.CompressionAlgorithms;
using BackupsExtra.CreationalAlgorithms;
using BackupsExtra.Entities;
using BackupsExtra.Logging;
using BackupsExtra.Repository;
using Moq;
using NUnit.Framework;
using File = Backups.Entities.File;

namespace BackupsExtra.Tests
{
    public class ClearAlgorithmsTests
    {
        private IRestorePointManageAlgorithm _restorePointCreationalAlgorithm;
        private IExtraCompressor _compressor;
        private Mock<ISourceRepository> _repository;
        private Mock<IExtraBackupDestinationRepository> _backupDestinationRepository;

        [SetUp]
        public void Setup()
        {
            _restorePointCreationalAlgorithm =
                new SingleStorageRestorePointManageAlgorithm();
            _compressor = new ExtraZipCompressor();
            _repository = new Mock<ISourceRepository>();
            var logger = new Mock<ILogger>();
            _backupDestinationRepository = new Mock<IExtraBackupDestinationRepository>();
            _repository.Setup(x => x
                    .ReadFile(It.IsAny<string>()))
                    .Returns(new File(new MemoryStream() ,"awesomeName"));
            _backupDestinationRepository.Setup(x => x
                .Write(It.IsAny<Storage>(), It.IsAny<Stream>()));
            _backupDestinationRepository.Setup(x => x
                .Delete(It.IsAny<Storage>()));
            _backupDestinationRepository.Setup(x => x
                .Read(It.IsAny<Storage>(), It.IsAny<Stream>()));
            _backupDestinationRepository.Setup(a => a.CreateStorage(It.IsAny<List<JobObjectWithData>>(), new ZipCompressor(), It.IsAny<string>()))
                .Returns((List<JobObject> x, IExtraCompressor z, string y) =>
                    new Storage("Awesome Name", y, Guid.NewGuid(), x));
            logger.Setup(x => x
                .Write(It.IsAny<LoggerMessage>()));
            LoggerSingletone.Initialize((logger.Object));
        }

        [Test]
        public void CountClearAlgorithm_ShouldDeletePoints()
        {
            var clearAlgorithmBuilder = new ClearAlgorithmBuilder();
            clearAlgorithmBuilder.AddMaxCount(3);
            IClearAlgorithm clearAlgorithm = clearAlgorithmBuilder.Build();
            var backup = new ExtraBackup(_restorePointCreationalAlgorithm, _compressor, _backupDestinationRepository.Object, clearAlgorithm, _repository.Object);
            var storage = new Storage("Awesome Name", "", Guid.NewGuid(), new List<JobObject>());
            backup.Add(new RestorePoint(new List<Storage>(){ storage }, DateTime.Now, Guid.NewGuid(), "awesomePath"));
            storage = new Storage("Awesome Name", "", Guid.NewGuid(), new List<JobObject>());
            backup.Add(new RestorePoint(new List<Storage>(){ storage }, DateTime.Now, Guid.NewGuid(), "awesomePath"));
            storage = new Storage("Awesome Name", "", Guid.NewGuid(), new List<JobObject>());
            backup.Add(new RestorePoint(new List<Storage>(){ storage }, DateTime.Now, Guid.NewGuid(), "awesomePath"));
            storage = new Storage("Awesome Name", "", Guid.NewGuid(), new List<JobObject>());
            backup.Add(new RestorePoint(new List<Storage>(){ storage }, DateTime.Now, Guid.NewGuid(), "awesomePath"));
            storage = new Storage("Awesome Name", "", Guid.NewGuid(), new List<JobObject>());
            backup.Add(new RestorePoint(new List<Storage>(){ storage }, DateTime.Now, Guid.NewGuid(), "awesomePath"));
            backup.Clear();
            Assert.AreEqual(3, backup.RestorePoints.Count);
        }

        [Test]
        public void DateClearAlgorithm_ShouldDeletePoints()
        {
            var clearAlgorithmBuilder = new ClearAlgorithmBuilder();
            clearAlgorithmBuilder.AddMaxTime(new TimeSpan(1,0,0));
            IClearAlgorithm clearAlgorithm = clearAlgorithmBuilder.Build();
            var backup = new ExtraBackup(_restorePointCreationalAlgorithm, _compressor, _backupDestinationRepository.Object, clearAlgorithm, _repository.Object);
            var storage = new Storage("Awesome Name", "", Guid.NewGuid(), new List<JobObject>());
            backup.Add(new RestorePoint(new List<Storage>(){ storage }, DateTime.Now, Guid.NewGuid(), "awesomePath"));
            storage = new Storage("Awesome Name", "", Guid.NewGuid(), new List<JobObject>());
            backup.Add(new RestorePoint(new List<Storage>(){ storage }, DateTime.Now - TimeSpan.FromDays(2), Guid.NewGuid(), "awesomePath"));
            storage = new Storage("Awesome Name", "", Guid.NewGuid(), new List<JobObject>());
            backup.Add(new RestorePoint(new List<Storage>(){ storage }, DateTime.Now - TimeSpan.FromDays(2), Guid.NewGuid(), "awesomePath"));
            storage = new Storage("Awesome Name", "", Guid.NewGuid(), new List<JobObject>());
            backup.Add(new RestorePoint(new List<Storage>(){ storage }, DateTime.Now - TimeSpan.FromDays(2), Guid.NewGuid(), "awesomePath"));
            storage = new Storage("Awesome Name", "", Guid.NewGuid(), new List<JobObject>());
            backup.Add(new RestorePoint(new List<Storage>(){ storage }, DateTime.Now - TimeSpan.FromDays(2), Guid.NewGuid(), "awesomePath"));
            backup.Clear();
            Assert.AreEqual(1, backup.RestorePoints.Count);
        }
        
        [Test]
        public void DateClearAlgorithm_NothingRemainsError()
        {
            var clearAlgorithmBuilder = new ClearAlgorithmBuilder();
            clearAlgorithmBuilder.AddMaxTime(new TimeSpan(1,0,0));
            IClearAlgorithm clearAlgorithm = clearAlgorithmBuilder.Build();
            var backup = new ExtraBackup(_restorePointCreationalAlgorithm, _compressor, _backupDestinationRepository.Object, clearAlgorithm, _repository.Object);
            var storage = new Storage("Awesome Name", "", Guid.NewGuid(), new List<JobObject>());
            backup.Add(new RestorePoint(new List<Storage>(){ storage }, DateTime.Now - TimeSpan.FromDays(2), Guid.NewGuid(), "awesomePath"));
            Assert.Catch<InvalidOperationException>( () => backup.Clear());
        }
    }
}