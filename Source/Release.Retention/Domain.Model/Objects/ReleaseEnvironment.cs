namespace Release.Retention.Domain.Model.Objects
{
    public class ReleaseEnvironment
    {
        public string Id { get; private set; }
        public string Name { get; private set; }

        public ReleaseEnvironment(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}