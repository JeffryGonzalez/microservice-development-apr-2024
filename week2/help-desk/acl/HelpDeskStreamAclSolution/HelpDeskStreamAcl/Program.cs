
using HelpDeskStreamAcl.Outgoing;
using JasperFx.Core;
using Marten;
using Oakton.Resources;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.Kafka;
using Wolverine.Marten;
using Wolverine.Postgresql;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("data") ?? throw new Exception("No Connection String");
var kafkaConnectionString = builder.Configuration.GetConnectionString("kafka") ?? throw new Exception("No Broker");

builder.Services.AddMarten(options =>
{
    options.Connection(connectionString);

}).UseLightweightSessions().IntegrateWithWolverine();

builder.Host.UseWolverine(opts =>
{
    opts.PersistMessagesWithPostgresql(connectionString, "wolverine");
    opts.Policies.UseDurableLocalQueues();
    opts.Policies.UseDurableInboxOnAllListeners();
    opts.Policies.UseDurableOutboxOnAllSendingEndpoints();
    opts.UseKafka(kafkaConnectionString).ConfigureConsumers(c =>
    {
        c.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
        c.GroupId = "help-desk-stream-acl";

    });

    opts.PublishMessage<IssueDocument>().ToKafkaTopic("help-desk-stream-acl.issue");
    opts.ListenToKafkaTopic("softwarecenter.catalog-item-created").ProcessInline();
    opts.ListenToKafkaTopic("help-desk.issue-created").ProcessInline();
    opts.Services.AddResourceSetupOnStartup();
    opts.OnException<RaceConditionException>().RetryWithCooldown(50.Milliseconds(), 100.Milliseconds(), 1000.Milliseconds());
});



var app = builder.Build();




app.Run();
public class RaceConditionException : ArgumentOutOfRangeException { }
