namespace Hangf.Entities
{
    public class HangfireServer
    {
        public string? Name { get; set; }
        public int WorkerCount { get; set; }
        public string[]? QueueList { get; set; }
    }
}
