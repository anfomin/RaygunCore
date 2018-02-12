using System;
using System.Diagnostics;

namespace RaygunCore.Services
{
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
			if (exception == null)
				throw new ArgumentNullException(nameof(exception));
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
			if (exception == null)
				throw new ArgumentNullException(nameof(exception));
			return exception?.Data != null && exception.Data.Contains(SentKey) && exception.Data[SentKey] is bool b && b;
		}
	}
}