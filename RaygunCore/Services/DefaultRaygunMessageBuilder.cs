using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using RaygunCore.Messages;

namespace RaygunCore.Services;

/// <summary>
/// Provides <see cref="RaygunMessage"/> building using providers <see cref="IRaygunMessageProvider"/>.
/// </summary>
public class DefaultRaygunMessageBuilder : IRaygunMessageBuilder
{
	static Regex[] DiagnosticsMessages =
	{
		new("^An unhandled exception has occurred while executing the request.$", RegexOptions.IgnoreCase),
		new(@"^Connection ID ""[\w:-]+"", Request ID ""[\w:-]+"": An unhandled exception was thrown by the application.$", RegexOptions.IgnoreCase)
	};
	readonly IEnumerable<IRaygunMessageProvider> _providers;

	public DefaultRaygunMessageBuilder(IEnumerable<IRaygunMessageProvider> providers)
		=> _providers = providers ?? throw new ArgumentNullException(nameof(providers));

	/// <inheritdoc/>
	public virtual RaygunMessage Build(string message, Exception? exception, RaygunSeverity? severity, IEnumerable<string>? tags, IDictionary<string, object>? customData)
	{
		ArgumentNullException.ThrowIfNull(message);

		// create message
		var msg = new RaygunMessage();
		if (severity != null)
			msg.Details.Tags.Add(severity.Value.ToString());
		foreach (var provider in _providers)
			provider.Apply(msg.Details);

		if (tags != null)
			msg.Details.Tags.AddRange(tags);
		if (customData != null)
		{
			foreach (var kv in customData)
				msg.Details.UserCustomData[kv.Key] = kv.Value;
		}

		// apply message or exception
		if (exception == null || ShouldApplyCustomErrorMessage(message, exception))
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
			else
				msg.Details.Error.StackTrace = BuildStackTrace();
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
			message.Data = new();
			foreach (object key in exception.Data.Keys)
			{
				if (!ExceptionExtensions.SentKey.Equals(key))
					message.Data[key] = exception.Data[key];
			}
		}

		if (exception is AggregateException ae && ae.InnerExceptions != null)
			message.InnerErrors = ae.InnerExceptions.Select(ex => CreateErrorMessage(ex)).ToArray();
		else if (exception.InnerException != null)
			message.InnerError = CreateErrorMessage(exception.InnerException);

		return message;
	}

	RaygunErrorStackTraceLineMessage[] BuildStackTrace()
		=> BuildStackTrace(new StackTrace(true), ignoreRaygunCore: true);

	RaygunErrorStackTraceLineMessage[] BuildStackTrace(Exception exception)
		=> BuildStackTrace(new StackTrace(exception, true));

	RaygunErrorStackTraceLineMessage[] BuildStackTrace(StackTrace stackTrace, bool ignoreRaygunCore = false)
	{
		var frames = stackTrace.GetFrames();
		if (frames == null || frames.Length == 0)
			return new[] { new RaygunErrorStackTraceLineMessage { FileName = "none", LineNumber = 0 } };

		var lines = new List<RaygunErrorStackTraceLineMessage>();
		foreach (var frame in frames)
		{
			var method = frame.GetMethod();
			if (method == null || ignoreRaygunCore && method.DeclaringType?.Namespace?.StartsWith("RaygunCore") == true)
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
		=> message != exception.Message && DiagnosticsMessages.All(regex => !regex.IsMatch(message));

	string GenerateMethodName(MethodBase method)
	{
		var sb = new StringBuilder(method.Name);

		if (method is MethodInfo && method.IsGenericMethod)
		{
			sb.Append("<");
			sb.Append(string.Join(",", method.GetGenericArguments().Select(a => a.Name)));
			sb.Append(">");
		}

		sb.Append("(");
		sb.Append(string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType?.Name ?? "<UnknownType>"} {p.Name}")));
		sb.Append(")");
		return sb.ToString();
	}
}