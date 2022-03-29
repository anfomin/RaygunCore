namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides methods to register Raygun services.
/// </summary>
public interface IRaygunBuilder
{
	/// <summary>
	/// Gets services collection.
	/// </summary>
	IServiceCollection Services { get; }

	/// <summary>
	/// Gets if builder is registering services for logging.
	/// </summary>
	bool IsLogging { get; }
}

internal class RaygunBuilder : IRaygunBuilder
{
	public IServiceCollection Services { get; }
	public bool IsLogging { get; internal set; }

	public RaygunBuilder(IServiceCollection services)
		=> Services = services ?? throw new ArgumentNullException(nameof(services));
}