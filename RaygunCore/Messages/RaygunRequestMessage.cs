namespace RaygunCore.Messages;

public class RaygunRequestMessage
{
	public string? HostName { get; set; }
	public string? Url { get; set; }
	public string? HttpMethod { get; set; }
	public string? IPAddress { get; set; }
	public Dictionary<string, object>? Headers { get; set; }
	public Dictionary<string, object>? Cookies { get; set; }
	public Dictionary<string, object>? QueryString { get; set; }
	public Dictionary<string, object>? Form { get; set; }
	public Dictionary<string, object>? Data { get; set; }
	public string? RawData { get; set; }
}