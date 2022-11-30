using Hangfire;

namespace Hangf.Jobs.Interfaces
{
    public interface ISendMailJob
    {
        [AutomaticRetry(Attempts = 6)]
        [Queue("mail")]
        void SendMail(string mailName);
    }
}
