using Microsoft.EntityFrameworkCore;
using SimpleRateLimiter.Models;
using SimpleRateLimiter.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
[assembly: ApiController]

namespace SimpleRateLimiter;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        builder.Services.AddControllers();

        builder.Services.AddDbContext<BucketContext>(opt =>
            opt.UseInMemoryDatabase("EndpointBucket"));

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: MyAllowSpecificOrigins,
            policy =>
            {
                policy
                    .WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        InitializeDatabase();

        builder.Services.AddScoped<IBucketManager, BucketManager>();
        builder.Services.AddHostedService<TokenTimer>();

        var app = builder.Build();

        app.UseCors(MyAllowSpecificOrigins);
        app.MapControllers();

        app.Run();
    }

    public static void InitializeDatabase()
    {
        var options = new DbContextOptionsBuilder<BucketContext>()
                    .UseInMemoryDatabase(databaseName: "EndpointBucket")
                    .Options;

        var config = JsonConvert.DeserializeObject<IList<EndpointConfig>>(File.ReadAllText(@"endpoint.config.json"));

        if (config != null)
        {
            var context = new BucketContext(options);
            context.Database.EnsureDeleted();
            foreach (var endpoint in config)
            {
                context.EndpointBuckets.Add(new EndpointBucket { Endpoint = endpoint.Endpoint, Tokens = endpoint.Burst });
            }

            context.SaveChanges();
        }
    }
}
