//using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Debugging;
//using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.File;
using Serilog.Sinks.Http.BatchFormatters;
//using Serilog.Sinks.Elasticsearch.Durable;
using Serilog.Sinks.SystemConsole.Themes;

//using Elastic.CommonSchema.Serilog;
using Elastic.Serilog.Sinks;
using Elastic.Channels;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Ingest.Elasticsearch;
using Elastic.Transport;

//using Serilog.Sinks;
//using Serilog.Extensions;
//using Elastic.Serilog.Sinks;
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

//Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine($"Serilog: {msg}"));
builder.Logging.ClearProviders();
SelfLog.Enable(Console.Error);
Log.Logger = new LoggerConfiguration()
.MinimumLevel.Debug()


    .WriteTo.Console(theme: SystemConsoleTheme.Literate)
    .Enrich.FromLogContext()

    .WriteTo.Elasticsearch(new[] { new Uri("http://localhost:9200") }, opts =>
    {
        opts.BootstrapMethod = BootstrapMethod.Failure;
        opts.DataStream = new DataStreamName("logs", "console-example");
        opts.ConfigureChannel = channelOpts =>
        {
            channelOpts.BufferOptions = new BufferOptions { ExportMaxConcurrency = 10 };
        };
    }, tranportation =>
    {
        tranportation.Authentication(new BasicAuthentication("elastic", "changeme"));
    })

    // working with logstash over http
    //.WriteTo.DurableHttpUsingFileSizeRolledBuffers(requestUri: "http://localhost:31311")

    .CreateLogger();


Log.Information("Hello, world!");

//var logger = new LoggerConfiguration()
//  .Enrich.FromLogContext()
//  //.WriteTo.Console()
//  .MinimumLevel.Debug()
//  //.WriteTo.File("output.json")
//  .WriteTo.Elasticsearch(new Serilog.Sinks.Elasticsearch.ElasticsearchSinkOptions(new Uri("http://elastic:changeme@localhost:9200"))
//  {
//      IndexFormat = $"logs",
//      AutoRegisterTemplate = true,
//      FailureCallback = e =>
//      {
//          Console.WriteLine("failure");
//      },
//      EmitEventFailure = Serilog.Sinks.Elasticsearch.EmitEventFailureHandling.ThrowException,
//      AutoRegisterTemplateVersion = Serilog.Sinks.Elasticsearch.AutoRegisterTemplateVersion.ESv8,
//      ModifyConnectionSettings = (e) => e.BasicAuthentication("elastic", "changeme")
//  })
//.CreateBootstrapLogger();




//{
//  new Uri("http://elastic:changeme@localhost:9200") },
//  opts =>
//  {
//    opts.MinimumLevel = Serilog.Events.LogEventLevel.Information;
//    opts.DataStream = new Elastic.Ingest.Elasticsearch.DataStreams.DataStreamName($"logs-{builder.Configuration["APP_NAME"]}-{DateTime.UtcNow:yyyy-mm-dd}");
//  }

//
builder.Host.UseSerilog();
//builder.(logger);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//
app.UseSerilogRequestLogging();
//
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};


app.MapGet("/weatherforecast", () =>
{
    var rng = new Random();
    //_logger.Error("First Log Message");

    var randomNum = rng.Next();
    Log.Error("first log after format change");
    Log.Information("Random number logged: {randomNum}", randomNum);
    randomNum = rng.Next();
    Log.Information("Random number logged: {randomNum}", randomNum);

    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    //Log.CloseAndFlush();
    //logger.Information("logger dependency");
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

static Task<string> foo()
{
    return Task.FromResult("test");
}
internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
