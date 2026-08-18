namespace ManagerServer.Services
{
    public sealed class IdleTracker
    {
        public DateTime LastEvent { get; private set; } = DateTime.UtcNow;

        public TimeSpan Idle => DateTime.UtcNow - LastEvent;

        public void MarkActivity()
        {
            LastEvent = DateTime.UtcNow;
        }
    }
}
