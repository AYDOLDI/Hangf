using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Hangf;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Extensions;
using System.Collections.Generic;
using Hangf.Entities;
using Hangf.Jobs.Implementations;
using Hangf.Jobs.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

//ssl certificate   

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString1 = builder.Configuration.GetConnectionString("DefaultConnection");
var connectionString2 = builder.Configuration.GetConnectionString("HangfireConnection");

builder.Services.AddDbContext<ApplicationDbContext>(x => x.UseSqlServer(connectionString1));


builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(connectionString2, new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true
        })

        );


//i can use it for recurring jobs based on this time service.
//builder.Services.AddTransient<ITimeService, TimeService>();

builder.Services.AddHangfireServer();
var serverList = builder.Configuration.GetSection("HangfireSettings:ServerList").Get<List<HangfireServer>>();

foreach (var server in serverList)
{
    builder.Services.AddHangfireServer(option =>
    {
        option.ServerName = server.Name;
        option.WorkerCount = server.WorkerCount;
        option.Queues = server.QueueList;
        //bu şekilde saniyede bir fetch edebilir. ama akıllıca bir yöntem değil. çünkü bottleneck yaşanabilir database için. saniyede bir query oluşturuyor cunku
        option.SchedulePollingInterval = TimeSpan.FromSeconds(15);
    });
}



builder.Services.AddSingleton<INotSetAttemptCountJob, NotSetAttemptCountJob>();
builder.Services.AddSingleton<ISendMailJob, SendMailJob>();
builder.Services.AddSingleton<ICpuCounterJob, CpuCounterJob>();





//builder.Services.AddHangfireServer();

/*builder.Services.AddHangfireServer(option =>
{
    option.ServerName = "mail";
    option.WorkerCount = 5;
    option.CancellationCheckInterval = TimeSpan.FromSeconds(10);
    option.Queues = new[] { "general", "mail" };
});
builder.Services.AddHangfireServer(option =>
{
    option.ServerName = "integration";
    option.WorkerCount = 10;
    option.CancellationCheckInterval = TimeSpan.FromSeconds(10);
    option.Queues = new[] { "general", "integration" };
});
builder.Services.AddHangfireServer(option =>
{
    option.ServerName = "excel";
    option.WorkerCount = 8;
    option.CancellationCheckInterval = TimeSpan.FromSeconds(10);
    option.Queues = new[] { "general", "product", "customer" };
});*/

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseAuthorization();


//authorization
var options = new DashboardOptions
{
    AppPath = "https://google.com",
    DashboardTitle = "Hangfire Demo Dashboard",
    Authorization = new[] { new HangfireCustomBasicAuthenticationFilter {
        User = builder.Configuration.GetSection("HangfireSettings:Username").Value,
        Pass = builder.Configuration.GetSection("HangfireSettings:Password").Value} }
};



//hangfire adresinden gir ve hangfire dashboard'ını aç, hangfire dashboard'ı için hangfire settings'ını kullan (optionsı)
app.UseHangfireDashboard("/hangfire", options);
//app.UseHangfireDashboard();
app.MapHangfireDashboard();

//bunlar buraya yazılmalı çünkü biz aplikasyonu load ettiğimizde bu fonksiyon otomatik olarak recurring job olarak konfigüre ediliyor. yani bu demek ki her zaman en az 1 defa alısacak
//RecurringJob.AddOrUpdate<ITimeService>("print-time", service => service.PrintNow(), Cron.Minutely); //Cron.Daily,Hourly or "5 * * * *"

app.MapControllers();

app.Run();

//can be used when follow official doc
/*public class MyAuthorizationFilter : IDashboardAuthorizationFilter
{
    public string User { get; set; }
    public string Pass { get; set; }

   public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        var user = httpContext.Request.Query["user"];
        var pass = httpContext.Request.Query["pass"];
        return user == User && pass == Pass;
    }
}*/

//startupta cronjovstarter var dk 21.38