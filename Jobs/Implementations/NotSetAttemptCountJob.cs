using Hangf.Jobs.Interfaces;
using Hangfire;

namespace Hangf.Jobs.Implementations
{
    public class NotSetAttemptCountJob: INotSetAttemptCountJob
    {
        public void Run()
        {
            throw new System.NotImplementedException("It will be retried for 5 times by default");
        }
    }
}
