using Hangf.Jobs.Interfaces;

namespace Hangf.Jobs.Implementations
{
    public class DeleteUserJob : IDeleteUserJob
    {
        private readonly ILogger<DeleteUserJob> logger;

        public DeleteUserJob(ILogger<DeleteUserJob> logger)
        {
            this.logger = logger;
        }
        public Task DeleteIt(string username, string mail)
        {
            logger.LogInformation($"Sending {mail} mail to {username} to confirm deleting");
            return Task.CompletedTask;
        }
    }
}
