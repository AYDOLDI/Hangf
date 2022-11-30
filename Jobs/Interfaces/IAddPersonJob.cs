using Hangfire;

namespace Hangf.Jobs.Interfaces
{
    public interface IAddPersonJob
    {
        [AutomaticRetry(Attempts = 5)]
        [Queue("integration")]
        void CreatePerson(string personName);
    }
}
