namespace TestApp.Data
{
    public class UserRow
    {
        public UserRow(string name)
        {
            Name = name;
        }
        public string Name { get; }
        public string PbClass { get; set; } = "progress-bar bg-success";
        public double Progress { get; set; } = 0;
        public double Index { get; set; } = 0;
        public string Description { get; set; } = string.Empty;
    }
}
