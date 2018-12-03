using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using RaygunCore.Messages;

namespace RaygunCore.Services
{
	/// <summary>
	/// Provides <see cref="RaygunMessage"/> building using providers <see cref="IRaygunMessageProvider"/>.
	/// </summary>
	public class DefaultRaygunMessageBuilder : IRaygunMessageBuilder
	{
		static Regex[] DiagnosticsMessages =
		{
			new Regex("^An unhandled exception has occurred while executing the request.$", RegexOptions.IgnoreCase),
			new Regex(@"^Connection id ""\w+"", Request id ""\w+:\w+"": An unhandled exception was thrown by the application.$", RegexOptions.IgnoreCase)
		};
		readonly IEnumerable<IRaygunMessageProvider> _providers;

		public DefaultRaygunMessageBuilder(IEnumerable<IRaygunMessageProvider> providers)
		{
			_providers = providers ?? throw new ArgumentNullException(nameof(providers));
		}

		/// <inheritdoc/>
		public virtual RaygunMessage Build(string message, Exception exception, RaygunSeverity? severity, IList<string> tags, IDictionary<string, object> customData)
		{
			if (message == null && exception == null)
				throw new ArgumentNullException(nameof(message), "Message or exception is required");

			// create message
			var msg = new RaygunMessage();
			if (severity != null)
				msg.Details.Tags.Add(severity.Value.ToString());
			foreach (var p in _providers)
				p.Apply(msg.Details);

			if (tags != null)
				msg.Details.Tags.AddRange(tags);
			if (customData != null)
			{
				foreach (var kv in customData)
					msg.Details.UserCustomData[kv.Key] = kv.Value;
			}

			// apply message or exception
			if (message != null && (exception == null || ShouldApplyCustomErrorMessage(message, exception)))
			{
				msg.Details.Error = new RaygunErrorMessage
				{
					ClassName = (severity ?? RaygunSeverity.Error).ToString(),
					Message = message
				};
				if (exception != null)
				{
					msg.Details.Error.StackTrace = BuildStackTrace(exception);
					msg.Details.Error.InnerError = CreateErrorMessage(exception);
				}
			}
			else
				msg.Details.Error = CreateErrorMessage(exception);
			return msg;
		}

		/// <summary>
		/// Creates <see cref="RaygunErrorMessage"/> for exception.
		/// </summary>
		protected RaygunErrorMessage CreateErrorMessage(Exception exception)
		{
			var message = new RaygunErrorMessage
			{
				ClassName = exception.GetType().FullName,
				Message = exception.Message,
				StackTrace = BuildStackTrace(exception)
			};

			if (exception.Data != null)
			{
				var data = new Dictionary<object, object>();
				foreach (object key in exception.Data.Keys)
				{
					if (!ExceptionExtensions.SentKey.Equals(key))
						data[key] = exception.Data[key];
				}
				message.Data = data;
			}

			if (exception is AggregateException ae && ae.InnerExceptions != null)
				message.InnerErrors = ae.InnerExceptions.Select(ex => CreateErrorMessage(ex)).ToArray();
			else if (exception.InnerException != null)
				message.InnerError = CreateErrorMessage(exception.InnerException);

			return message;
		}

		RaygunErrorStackTraceLineMessage[] BuildStackTrace(Exception exception)
		{
			var stackTrace = new StackTrace(exception, true);
			var frames = stackTrace.GetFrames();
			if (frames == null || frames.Length == 0)
				return new[] { new RaygunErrorStackTraceLineMessage { FileName = "none", LineNumber = 0 } };

			var lines = new List<RaygunErrorStackTraceLineMessage>();
			foreach (var frame in frames)
			{
				var method = frame.GetMethod();
				if (method == null)
					continue;

				int lineNumber = frame.GetFileLineNumber();
				if (lineNumber == 0)
					lineNumber = frame.GetILOffset();

				lines.Add(new RaygunErrorStackTraceLineMessage
				{
					FileName = frame.GetFileName(),
					LineNumber = lineNumber,
					ClassName = method.DeclaringType?.FullName ?? "(unknown)",
					MethodName = GenerateMethodName(method)
				});
			}

			return lines.ToArray();
		}

		bool ShouldApplyCustomErrorMessage(string message, Exception exception)
		{
			return message != exception.Message && DiagnosticsMessages.All(regex => !regex.IsMatch(message));
		}

		string GenerateMethodName(MethodBase method)
		{
			var sb = new StringBuilder(method.Name);

			if (method is MethodInfo && method.IsGenericMethod)
			{
				sb.Append("<");
				sb.Append(String.Join(",", method.GetGenericArguments().Select(a => a.Name)));
				sb.Append(">");
			}

			sb.Append("(");
			sb.Append(String.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType?.Name ?? "<UnknownType>"} {p.Name}")));
			sb.Append(")");
			return sb.ToString();
		}
	}
}