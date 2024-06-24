using System.Reflection;
using RaygunCore.Messages;
using Microsoft.Extensions.Options;

namespace RaygunCore.Services;

/// <summary>
/// Provides <see cref="RaygunMessageDetails"/> with client information, machine name and application version.
/// </summary>
public class MainMessageProvider(IOptions<RaygunOptions> options) : IRaygunMessageProvider
{
	readonly RaygunOptions _options = options.Value;

	/// <inheritdoc/>
	public void Apply(RaygunMessageDetails details)
	{
		var asm = GetType().Assembly;
		details.MachineName = System.Environment.MachineName;
		details.Version = _options.AppVersion ?? Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
		details.Client.Name = asm.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
		details.Client.Version = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
		details.Client.ClientUrl = "https://github.com/anfomin/raygun";
	}
}