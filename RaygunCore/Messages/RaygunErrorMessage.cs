using System.Collections;

namespace RaygunCore.Messages;

public class RaygunErrorMessage
{
	public string? ClassName { get; set; }
	public string Message { get; set; } = "";
	public IDictionary? Data { get; set; }
	public RaygunErrorMessage? InnerError { get; set; }
	public RaygunErrorMessage[]? InnerErrors { get; set; }
	public RaygunErrorStackTraceLineMessage[]? StackTrace { get; set; }
}