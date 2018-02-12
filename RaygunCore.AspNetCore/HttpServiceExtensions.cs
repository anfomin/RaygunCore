using RaygunCore;
using RaygunCore.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	/// <see cref="IServiceCollection"/> extension methods for Raygun services registration.
	/// </summary>
	public static class RaygunHttpServiceExtensions
	{
		/// <summary>
		/// Registers Raygun message providers for <see cref="HttpContext"/>.
		/// </summary>
		public static IRaygunBuilder WithHttp(this IRaygunBuilder builder)
		{
			builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			builder.Services.AddSingleton<IStartupFilter, RaygunStartupFilter>();
			builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IRaygunMessageProvider, RequestMessageProvider>());
			builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IRaygunMessageProvider, ResponseMessageProvider>());
			builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IRaygunMessageProvider, UserMessageProvider>());
			builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IRaygunValidator, LocalValidator>());
			return builder;
		}
	}
}