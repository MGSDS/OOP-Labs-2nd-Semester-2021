using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Backups.CompressionAlgorithms;
using Backups.CreationalAlgorithms;
using Backups.Entities;
using Backups.Repositories;
using Moq;
using NUnit.Framework;
using File = Backups.Entities.File;

namespace Backups.Tests
{
    public class BackupJobTests
    {
        private Mock<IBackupDestinationRepository> _backupRepository;
        private Mock<IRepository> _repository;

        [SetUp]
        public void SetUp()
        {
            _repository = new Mock<IRepository>();
            _repository.Setup(a => a.ReadFile(It.IsAny<string>())).Returns(new File(new MemoryStream(), "awesome name"));
            _backupRepository = new Mock<IBackupDestinationRepository>();
            _backupRepository.Setup(a => a.CreateStorage(It.IsAny<List<JobObjectWithData>>(), new ZipCompressor(), It.IsAny<string>()))
                .Returns((List<JobObject> x, ICompressor z, string y) =>
                    new Storage("Awesome Name", y, Guid.NewGuid(), x));
        }

        [Test]
        public void
            CreateMultipleFileStorageJobAdd2FilesCreateRestorePointAdd1FileCreateRestorePoint_Created2RestorePointsAnd3Storages()
        {
            var joba = new BackupJob(new Backup(new SplitStorageRestorePointCreationalAlgorithm(), new ZipCompressor(), _backupRepository.Object, _repository.Object), new List<JobObject>());
            joba.AddObject(new JobObject("awesome path", "awesome name"));
            joba.AddObject(new JobObject("awesome path", "awesome name2"));
            joba.AddObject(new JobObject("awesome path", "awesome name3"));
            joba.CreateRestorePoint();
            joba.RemoveObject(joba.JobObjects[2]);
            joba.CreateRestorePoint();
            Assert.AreEqual(2, joba.Backup.RestorePoints.Count);
            Assert.AreEqual(3, joba.Backup.RestorePoints[0].Storages.Count);
            Assert.AreEqual(2, joba.Backup.RestorePoints[1].Storages.Count);
        }
    }
}