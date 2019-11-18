using System.Collections.Generic;

namespace RaygunCore.Messages
{
	public class RaygunRequestMessage
	{
		public string? HostName { get; set; }

		public string? Url { get; set; }

		public string? HttpMethod { get; set; }

		public string? IPAddress { get; set; }

		public IDictionary<string, object>? Headers { get; set; }

		public IDictionary<string, object>? Cookies { get; set; }

		public IDictionary<string, object>? QueryString { get; set; }

		public IDictionary<string, object>? Form { get; set; }

		public IDictionary<string, object>? Data { get; set; }

		public string? RawData { get; set; }
	}
}