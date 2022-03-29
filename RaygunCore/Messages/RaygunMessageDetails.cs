namespace RaygunCore.Messages;

public class RaygunMessageDetails
{
	public string? MachineName { get; set; }
	public string? GroupingKey { get; set; }
	public string? Version { get; set; }
	public RaygunClientMessage Client { get; } = new RaygunClientMessage();
	public RaygunEnvironmentMessage Environment { get; } = new RaygunEnvironmentMessage();
	public List<string> Tags { get; } = new List<string>();
	public Dictionary<string, object> UserCustomData { get; } = new Dictionary<string, object>();
	public RaygunErrorMessage Error { get; set; } = null!;
	public RaygunUserMessage? User { get; set; }
	public RaygunRequestMessage? Request { get; set; }
	public RaygunResponseMessage? Response { get; set; }
}