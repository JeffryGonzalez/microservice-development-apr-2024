using IssueTrackerApi.Data;

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


builder.Services.AddDbContext<IssuesDataContext>(opts =>
{
    opts.UseNpgsql(connectionString);
   
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
using(var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IssuesDataContext>();
    db.Database.Migrate();
}

app.Run();
