namespace RaygunCore;

/// <summary>
/// Exception that occured when sending error to the Raygun.
/// </summary>
public class RaygunException(Exception innerException) : Exception("Error sending exception to Raygun", innerException)
{
}