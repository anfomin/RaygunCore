using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace RaygunCore.Services
{
	/// <summary>
	/// Preforms logging to Raygun via <see cref="IRaygunClient"/>.
	/// Works only with log level warning and higher.
	/// </summary>
	public class RaygunLogger : ILogger
	{
		static HashSet<Task> _runningTasks = new HashSet<Task>();
		public static IEnumerable<Task> RunningTasks => _runningTasks;

		readonly Lazy<IRaygunClient> _client;

		public RaygunLogger(Lazy<IRaygunClient> client)
		{
			_client = client ?? throw new ArgumentNullException(nameof(client));
		}

		/// <summary>
		/// Not implemeted.
		/// </summary>
		public IDisposable? BeginScope<TState>(TState state) => null;

		/// <inheritdoc/>
		public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Warning;

		/// <inheritdoc/>
		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			if (formatter == null)
				throw new ArgumentNullException(nameof(formatter));
			if (!IsEnabled(logLevel))
				return;

			var message = formatter(state, exception);
			if (!string.IsNullOrEmpty(message) || exception != null)
			{
				var task = _client.Value.SendAsync(message, exception, GetSeverity(logLevel));
				if (!task.IsCompleted)
					_runningTasks.Add(task);
				task.ContinueWith(task => _runningTasks.Remove(task));
			}
		}

		RaygunSeverity GetSeverity(LogLevel logLevel)
			=> logLevel switch
			{
				LogLevel.Warning => RaygunSeverity.Warning,
				LogLevel.Error => RaygunSeverity.Error,
				LogLevel.Critical => RaygunSeverity.Critical,
				_ => throw new NotSupportedException($"LogLevel {logLevel} is not supported")
			};
	}
}