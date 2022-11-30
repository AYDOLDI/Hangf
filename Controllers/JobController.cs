using Hangf.Jobs.Implementations;
using Hangf.Jobs.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace Hangf.Controllers
{
    [ApiController]
    [Route("api/Jobs")]
    public class JobController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IBackgroundJobClient backgroundJobClient;
        public JobController(ApplicationDbContext context,
           IBackgroundJobClient backgroundJobClient)
        {
            this.context = context;
            this.backgroundJobClient = backgroundJobClient;
        }

        //fire and forget job
        [HttpPost("CreateUser")]
        public Task CreatePerson(string personName)
        {
            backgroundJobClient.Enqueue<IAddPersonJob>(repository => repository.CreatePerson(personName));
            return Task.CompletedTask;
        }

        //delayed
        [HttpPost("Delayed")]
        public IActionResult CpuCount()
        {
            var JobId = BackgroundJob.Schedule<ICpuCounterJob>(job => job.CpuCount(), TimeSpan.FromMinutes(1));
            return Ok($"Great! The Delayed job with id: {JobId} has been added. The delayed mail has been scheduled to the user and will be sent within 1 minute.");
        }
        
        //recurring job
        [HttpPost("NotSetAttemptTest")]
        public Task NotSet()
        {
            //RecurringJob.AddOrUpdate<INotSetAttemptCountJob>(NotSetAttemptCountJob.JobId, job => job.Run(), NotSetAttemptCountJob.IntervalPattern, TimeZoneInfo.Local, NotSetAttemptCountJob.Queue);
            //id, method, cron, timezone, queue
            RecurringJob.AddOrUpdate<INotSetAttemptCountJob>(job => job.Run(), "0 */30 * * * ?", TimeZoneInfo.Local, queue: "integration");
            return Task.CompletedTask;
        }

        //recurring job
        [HttpPost("SendMail")]
        public IActionResult SendMail(string mail)
        {
            RecurringJob.AddOrUpdate<ISendMailJob>(job => job.SendMail(mail), Cron.Weekly);
            return Ok($"The recurring job has been scheduled for user with mail: {mail}.");
        }

        //continious
        [HttpPost("DeleteUser")]
        public IActionResult DeleteUser(string username, string mail)
        {
            //var jobId = BackgroundJob.Enqueue<IDeleteUserJob>(job => job.DeleteIt(username, mail), "0 */30 * * * ?");
            var jobId = BackgroundJob.Enqueue(() => DeleteUserData(username));
            BackgroundJob.ContinueJobWith(jobId, () => SendConfirmationMailUponDataDeletion(mail));
            return Ok($"OK - Data for user with username: {username} has been deleted and a confirmation has been sent to: {mail}");
        }

        [NonAction]
        public void DeleteUserData(string username)
        {
            // Implement logic to delete data here for a specific user
            Console.WriteLine($"Deleted data for user {username}");
        }
        
        [NonAction]
        public void SendConfirmationMailUponDataDeletion(string mail)
        {
            Console.WriteLine($"Successfully sent deletion confirmation to mail: {mail}");
        }


    }
}
