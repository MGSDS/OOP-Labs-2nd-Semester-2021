using System;
using Backups.Repositories;
using Newtonsoft.Json;

namespace Backups.Entities
{
    public class JobObjectWithData : IDisposable
    {
        public JobObjectWithData(JobObject jobObject, ISourceRepository sourceRepository)
        {
            JobObject = jobObject;
            File = sourceRepository.ReadFile(jobObject.FullPath);
        }

        [JsonConstructor]
        public JobObjectWithData(JobObject jobObject, File file)
        {
            JobObject = jobObject;
            File = file;
        }

        public JobObject JobObject { get; }
        public File File { get; }

        public void Dispose()
        {
            File?.Dispose();
        }
    }
}