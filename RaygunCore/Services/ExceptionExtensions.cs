namespace RaygunCore.Services;

/// <summary>
/// Provides exception methods for <see cref="Exception"/>.
/// </summary>
public static class ExceptionExtensions
{
	public const string SentKey = "RaygunSent";

	/// <summary>
	/// Marks exception as sent top Raygun.
	/// </summary>
	/// <returns><c>True</c> if flagged successfully. Otherwise <c>false</c>.</returns>
	public static bool MarkSent(this Exception exception)
	{
		ArgumentNullException.ThrowIfNull(exception);
		if (exception.Data == null)
			return false;

		try
		{
			exception.Data[SentKey] = true;
			return true;
		}
		catch
		{
			return false;
		}
	}

	/// <summary>
	/// Gets if exception is sent to Raygun.
	/// </summary>
	public static bool IsSent(this Exception exception)
	{
		ArgumentNullException.ThrowIfNull(exception);
		return exception.Data != null && exception.Data.Contains(SentKey) && exception.Data[SentKey] is bool b && b;
	}

	/// <summary>
	/// Returns inner exceptions if <paramref name="exception"/> is any of <paramref name="wrapperExceptionTypes"/>.
	/// </summary>
	/// <param name="wrapperExceptionTypes">Exception types to strip.</param>
	public static IEnumerable<Exception> StripWrapperExceptions(this Exception exception, IEnumerable<Type> wrapperExceptionTypes)
	{
		ArgumentNullException.ThrowIfNull(wrapperExceptionTypes);
		if (exception.InnerException != null && wrapperExceptionTypes.Any(type => exception.GetType() == type))
		{
			if (exception is AggregateException ae)
			{
				foreach (var inner in ae.InnerExceptions)
				foreach (var ex in StripWrapperExceptions(inner, wrapperExceptionTypes))
					yield return ex;
			}
			else
			{
				foreach (var ex in StripWrapperExceptions(exception.InnerException, wrapperExceptionTypes))
					yield return ex;
			}
		}
		else
		{
			yield return exception;
		}
	}
}