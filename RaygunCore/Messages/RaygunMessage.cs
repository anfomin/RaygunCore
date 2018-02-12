using System;

namespace RaygunCore.Messages
{
	public class RaygunMessage
	{
		public DateTime OccurredOn { get; set; } = DateTime.UtcNow;

		public RaygunMessageDetails Details { get; } = new RaygunMessageDetails();
	}
}