using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RaygunCore.Services
{
	/// <summary>
	/// When stopping waites for all <see cref="RaygunLogger"/> requests to finish.
	/// </summary>
	public class RaygunLoggerWaiter : IHostedService
	{
		readonly ILogger _logger;

		public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);

		public RaygunLoggerWaiter(ILogger<RaygunLoggerWaiter> logger) => _logger = logger;

		public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

		public Task StopAsync(CancellationToken cancellationToken)
		{
			var tasks = RaygunLogger.RunningTasks;
			if (!tasks.Any())
				return Task.CompletedTask;

			_logger.LogInformation($"Waiting for {tasks.Count()} Raygun requests to complete");
			return Task.WhenAny(
				Task.Delay(Timeout, cancellationToken),
				Task.WhenAll(RaygunLogger.RunningTasks)
			);
		}
	}
}