using System;

namespace RaygunCore
{
	/// <summary>
	/// Exception that occured when sending error to the Raygun.
	/// </summary>
	public class RaygunException : Exception
	{
		public RaygunException(Exception innerException) : base("Error sending exception to Raygun", innerException) { }
	}
}