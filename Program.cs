using Microsoft.AspNetCore.Mvc;
[assembly: ApiController]

// using Newtonsoft.Json;
// IList<Item> config = JsonConvert.DeserializeObject<IList<Item>>(File.ReadAllText(@"config.json"));

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
