using System;
using System.Collections.Generic;
using System.Linq;
using Backups.CompressionAlgorithms;
using Backups.CreationalAlgorithms;
using Backups.Entities;
using Backups.Repositories;
using Moq;
using NUnit.Framework;

namespace Backups.Tests
{
    public class BackupJobTests
    {
        private Mock<IRepository> _repository;

        [SetUp]
        public void SetUp()
        {
            _repository = new Mock<IRepository>();
            _repository.Setup(a => a.CreateStorage(It.IsAny<IReadOnlyList<JobObject>>(), It.IsAny<string>()))
                .Returns((List<JobObject> x, string y) =>
                    new Storage("Awesome Name", y, Guid.NewGuid(), x));
        }

        [Test]
        public void
            CreateMultipleFileStorageJobAdd2FilesCreateRestorePointAdd1FileCreateRestorePoint_Created2RestorePointsAnd3Storages()
        {
            var joba = new BackupJob(new Backup(new SplitStorageRestorePointCreationalAlgorithm(), new ZipCompressor(), _repository.Object), new List<JobObject>());
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