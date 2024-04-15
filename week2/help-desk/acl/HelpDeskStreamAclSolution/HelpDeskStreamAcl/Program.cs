
using Marten;
using Oakton.Resources;
using Wolverine;
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

    opts.ListenToKafkaTopic("softwarecenter.catalog-item-created").ProcessInline();
    opts.Services.AddResourceSetupOnStartup();
});



var app = builder.Build();





app.Run();
