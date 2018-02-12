using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace RaygunCore.Services
{
	/// <summary>
	/// Registers <see cref="RaygunMiddleware"/> to capture pipeline errors and send them to Raygun.
	/// </summary>
	public class RaygunStartupFilter : IStartupFilter
	{
		/// <inheritdoc/>
		public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
		{
			return app =>
			{
				app.UseMiddleware<RaygunMiddleware>();
				next(app);
			};
		}
	}
}