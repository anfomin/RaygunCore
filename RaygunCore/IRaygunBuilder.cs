using System;

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	/// Provides methods to register Raygun services.
	/// </summary>
	public interface IRaygunBuilder
	{
		/// <summary>
		/// Gets services collection.
		/// </summary>
		IServiceCollection Services { get; }
	}

	internal class RaygunBuilder : IRaygunBuilder
	{
		public IServiceCollection Services { get; }

		public RaygunBuilder(IServiceCollection services)
		{
			Services = services ?? throw new ArgumentNullException(nameof(services));
		}
	}
}