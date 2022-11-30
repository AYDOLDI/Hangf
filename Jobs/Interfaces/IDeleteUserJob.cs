using Hangfire;

namespace Hangf.Jobs.Interfaces
{
    public interface IDeleteUserJob
    {
        [AutomaticRetry(Attempts = 2)]
        [Queue("mail")]
        Task DeleteIt(string username, string mail);
    }
}
