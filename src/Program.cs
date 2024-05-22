using Microsoft.AspNetCore.Mvc;
[assembly: ApiController]

// using Newtonsoft.Json;
// IList<Item> config = JsonConvert.DeserializeObject<IList<Item>>(File.ReadAllText(@"config.json"));

namespace SimpleRateLimiter;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        var app = builder.Build();

        app.UseHttpsRedirection();
        app.MapControllers();

        app.Run();
    }
}
