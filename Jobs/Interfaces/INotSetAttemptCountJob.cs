using Hangfire;

namespace Hangf.Jobs.Interfaces
{
    public interface INotSetAttemptCountJob
    {
        //[AutomaticRetry(Attempts = 0, LogEvents = false, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [AutomaticRetry(Attempts = 0)]
        [Queue("integration")]
        void Run();
    }
}
