using Newtonsoft.Json;
using SimpleRateLimiter.Models;

namespace SimpleRateLimiter.Services;

public class TokenTimer(ILogger<TokenTimer> logger, IServiceScopeFactory scopeFactory) : BackgroundService
{
    private readonly ILogger<TokenTimer> _logger = logger;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly IList<EndpointConfig> _config = JsonConvert.DeserializeObject<IList<EndpointConfig>>(File.ReadAllText(@"endpoint.config.json")) ?? throw new InvalidOperationException("Endpoint config not found");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");

        using PeriodicTimer timer = new(TimeSpan.FromMilliseconds(500));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await DoWork();
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");
        }
    }

    private async Task DoWork()
    {
        _logger.LogInformation("Timed Hosted Service is working.");

        using (var scope = _scopeFactory.CreateScope())
        {
            foreach (var endpoint in _config)
            {
                var bucketManager = scope.ServiceProvider.GetRequiredService<IBucketManager>();
                var amount = endpoint.Sustained / 120;

                _logger.LogInformation("Adding {amount} tokens to {endpoint} (max={burst}).", amount, endpoint.Endpoint, endpoint.Burst);
                await bucketManager.RefillBucket(endpoint.Endpoint, amount, endpoint.Burst);
            }
        }
    }
}
