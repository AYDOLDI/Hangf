using Hangf.Entities;
using Hangf.Jobs.Interfaces;
using Hangfire;

namespace Hangf.Jobs.Implementations
{
    public class AddPersonJob: IAddPersonJob
    {
        private readonly ApplicationDbContext context;
        private readonly ILogger<AddPersonJob> logger;
        
        public AddPersonJob(ApplicationDbContext context, ILogger<AddPersonJob> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public void CreatePerson(string personName)
        {
            logger.LogInformation($"Adding person {personName}");
            var person = new Person { Name = personName };
            context.Add(person);       
            context.SaveChanges();
            logger.LogInformation($"Added the person {personName}");
        }
    }


    
}
