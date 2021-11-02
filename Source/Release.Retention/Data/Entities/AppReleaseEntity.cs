using System;

namespace Release.Retention.Data.Entities
{
    public class AppReleaseEntity
    {
        public string Id { get; set; }
        public string ProjectId { get; set; }
        public string Version { get; set; }
        public DateTime Created { get; set; }
    }
}