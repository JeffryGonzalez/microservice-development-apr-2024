using IssueTrackerApi.Data;
using IssueTrackerApi.Outgoing;
using Microsoft.EntityFrameworkCore;
using Oakton.Resources;
using Wolverine;
using Wolverine.Kafka;
using Wolverine.Postgresql;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("data") ?? throw new Exception("No Connection String");
var kafkaConnectionString = builder.Configuration.GetConnectionString("kafka") ?? throw new Exception("No Broker");

builder.Services.AddDbContext<IssuesDataContext>(opts =>
{
    opts.UseNpgsql(connectionString);

});

builder.Host.UseWolverine(opts =>
{
    opts.PersistMessagesWithPostgresql(connectionString, "wolverine");
    opts.Policies.UseDurableLocalQueues();
    opts.Policies.UseDurableInboxOnAllListeners();
    opts.Policies.UseDurableOutboxOnAllSendingEndpoints();
    opts.UseKafka(kafkaConnectionString).ConfigureConsumers(c =>
    {
        c.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest; // Earliest here means the earliest this consumer group hasn't already processed.
        // in Kafka, consumer groups are managed by the broker - it *knows*. 
        c.GroupId = "issue-tracker-api";

    });

    opts.ListenToKafkaTopic("softwarecenter.catalog-item-created")
        .ProcessInline();


    opts.ListenToKafkaTopic("softwarecenter.catalog-item-retired")
        .ProcessInline();

    opts.PublishMessage<IssueCreated>().ToKafkaTopic("help-desk.issue-created");
    opts.Services.AddResourceSetupOnStartup(); // sort of like the entity framework stuff to create the tables wolverine needs
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

// Note: Will auto apply the migrations - use cautiously.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IssuesDataContext>();
    db.Database.Migrate();
}

app.Run();
