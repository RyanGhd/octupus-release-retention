namespace Release.Retention.Domain.Model.Objects
{
    public class AppEnvironment
    {
        public string Id { get; private set; }
        public string Name { get; private set; }

        public AppEnvironment(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}