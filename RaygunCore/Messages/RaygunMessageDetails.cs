namespace RaygunCore.Messages;

public class RaygunMessageDetails
{
	public string? MachineName { get; set; }
	public string? GroupingKey { get; set; }
	public string? Version { get; set; }
	public RaygunClientMessage Client { get; set; } = new();
	public RaygunEnvironmentMessage Environment { get; set; } = new();
	public List<string> Tags { get; set; } = new();
	public Dictionary<string, object> UserCustomData { get; set; } = new();
	public RaygunErrorMessage Error { get; set; } = new();
	public RaygunUserMessage? User { get; set; }
	public RaygunRequestMessage? Request { get; set; }
	public RaygunResponseMessage? Response { get; set; }
}