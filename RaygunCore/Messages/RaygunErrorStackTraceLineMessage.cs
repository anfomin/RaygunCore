namespace RaygunCore.Messages;

public class RaygunErrorStackTraceLineMessage
{
	public required int LineNumber { get; set; }
	public required string ClassName { get; set; }
	public required string MethodName { get; set; }
	public string? FileName { get; set; }
}