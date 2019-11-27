using System;
using System.Collections.Generic;
using System.Reflection;

namespace RaygunCore
{
	/// <summary>
	/// Provides options for Raygun services.
	/// </summary>
	public class RaygunOptions
	{
		/// <summary>
		/// Gets or sets API key.
		/// </summary>
		public string ApiKey { get; set; } = null!;

		/// <summary>
		/// Gets or sets API endpoint.
		/// </summary>
		public Uri ApiEndpoint { get; set; } = new Uri("https://api.raygun.com/entries");

		/// <summary>
		/// Gets or sets a application version identifier for all error messages sent to the Raygun endpoint.
		/// </summary>
		public string? AppVersion { get; set; }

		/// <summary>
		/// Gets or sets if Raygun client should throw error if transmit failed.
		/// Default <c>false</c>.
		/// </summary>
		public bool ThrowOnError { get; set; }

		/// <summary>
		/// Gets set of outer exceptions that will be stripped, leaving only the valuable inner exception.
		/// This can be used when a wrapper exception, e.g. TargetInvocationException, contains the actual
		/// exception as the InnerException.
		/// </summary>
		public ISet<Type> WrapperExceptions { get; } = new HashSet<Type>
		{
			typeof(TargetInvocationException)
		};

		/// <summary>
		/// Gets or sets tags for all error messages sent to the Raygun endpoint.
		/// </summary>
		public IList<string>? Tags { get; set; }

		/// <summary>
		/// Gets or sets if errors for local requests are skipped.
		/// Works only when Raygun HTTP services registered.
		/// Default <c>false</c>.
		/// </summary>
		public bool IgnoreLocalErrors { get; set; }

		/// <summary>
		/// Gets or sets if <see cref="OperationCanceledException"/> is ignored in HTTP middleware.
		/// Default <c>true</c>.
		/// </summary>
		public bool IgnoreCanceledErrors { get; set; } = true;

		/// <summary>
		/// Gets or sets if request headers are not logged.
		/// Works only when Raygun HTTP services registered.
		/// Default <c>false</c>.
		/// </summary>
		public bool IgnoreHeaders { get; set; }

		/// <summary>
		/// Gets or sets if request cookies are not logged.
		/// Works only when Raygun HTTP services registered.
		/// Default <c>false</c>.
		/// </summary>
		public bool IgnoreCookies { get; set; }

		/// <summary>
		/// Gets or sets if request form values are not logged.
		/// Works only when Raygun HTTP services registered.
		/// Default <c>false</c>.
		/// </summary>
		public bool IgnoreForm { get; set; }

		/// <summary>
		/// Gets or sets a list of request header that are not logged.
		/// This allows you to remove sensitive data from the transmitted copy.
		/// Works only when Raygun HTTP services registered.
		/// </summary>
		public ISet<string> IgnoreHeaderNames { get; } = new HashSet<string>();

		/// <summary>
		/// Gets or sets a list of request cookies that are not logged.
		/// This allows you to remove sensitive data from the transmitted copy.
		/// Works only when Raygun HTTP services registered.
		/// </summary>
		public ISet<string> IgnoreCookieNames { get; } = new HashSet<string>();

		/// <summary>
		/// Gets or sets a list of request form fields that are not logged.
		/// This allows you to remove sensitive data from the transmitted copy.
		/// Works only when Raygun HTTP services registered.
		/// </summary>
		public ISet<string> IgnoreFormFields { get; } = new HashSet<string>();
	}
}
