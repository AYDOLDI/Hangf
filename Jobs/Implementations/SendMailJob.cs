using Hangf.Jobs.Interfaces;

namespace Hangf.Jobs.Implementations
{
    public class SendMailJob : ISendMailJob
    {
        private readonly ILogger<SendMailJob> logger;

        public SendMailJob(ILogger<SendMailJob> logger)
        {

            this.logger = logger;
        }
        public void SendMail(string mailName)
        {
            logger.LogInformation($"Sending mail {mailName}");
        }
    }
}
