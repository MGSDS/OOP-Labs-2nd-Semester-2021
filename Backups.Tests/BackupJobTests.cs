using System;
using System.Collections.Generic;
using System.Linq;
using Backups.CreationalAlgorithms;
using Backups.Entities;
using Backups.Repositories;
using Moq;
using NUnit.Framework;

namespace Backups.Tests
{
    public class BackupJobTests
    {
        private Mock<IRepository> _repo;

        [SetUp]
        public void SetUp()
        {
            _repo = new Mock<IRepository>();
            _repo.Setup(a => a.CreateStorages(It.IsAny<IReadOnlyList<JobObject>>(), It.IsAny<string>()))
                .Returns((List<JobObject> x, string y) =>
                    Enumerable.Repeat(new Storage("Awesome Name", y, Guid.NewGuid(), x), x.Count)
                        .ToList());
            _repo.Setup(a => a.CreateStorage(It.IsAny<IReadOnlyList<JobObject>>(), It.IsAny<string>()))
                .Returns((List<JobObject> x, string y) =>
                    new Storage("Awesome Name", y, Guid.NewGuid(), x));
        }

        [Test]
        public void
            CreateMultipleFileStorageJobAdd2FilesCreateRestorePointAdd1FileCreateRestorePoint_Created2RestorePointsAnd3Storages()
        {
            var joba = new BackupJob(new Backup(), _repo.Object, new List<JobObject>(),
                new SplitStorageRestorePointCreationalAlgorithm());
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