using System;
using System.Globalization;
using RaygunCore.Messages;

namespace RaygunCore.Services
{
	/// <summary>
	/// Provides <see cref="RaygunMessageDetails"/> with environment information.
	/// </summary>
	public class EnvironmentMessageProvider : IRaygunMessageProvider
	{
		/// <inheritdoc/>
		public void Apply(RaygunMessageDetails details)
		{
			details.Environment.Locale = CultureInfo.CurrentCulture.DisplayName;
			details.Environment.UtcOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).TotalHours;
			details.Environment.OSVersion = Environment.OSVersion.VersionString;
			details.Environment.ProcessorCount = Environment.ProcessorCount;
			details.Environment.Architecture = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
		}
	}
}