using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RaygunCore.Services;

/// <summary>
/// When stopping waites for all <see cref="RaygunLogger"/> requests to finish.
/// </summary>
public class RaygunLoggerWaiter(ILogger<RaygunLoggerWaiter> logger) : IHostedService
{
	readonly ILogger _logger = logger;

	public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);

	public Task StartAsync(CancellationToken cancellationToken)
		=> Task.CompletedTask;

	public Task StopAsync(CancellationToken cancellationToken)
	{
		var tasks = RaygunLogger.RunningTasks;
		if (!tasks.Any())
			return Task.CompletedTask;

		_logger.LogInformation("Waiting for {TaskCount} Raygun requests to complete", tasks.Count());
		return Task.WhenAny(
			Task.Delay(Timeout, cancellationToken),
			Task.WhenAll(RaygunLogger.RunningTasks)
		);
	}
}