using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using RaygunCore.Messages;
using RaygunCore.Services;

namespace RaygunCore.Test;

public class RaygunClientTest
{
	[Fact]
	public async Task Index()
	{
		string serverUrl = "http://localhost:8888/";
		var server = new HttpListener();
		server.Prefixes.Add(serverUrl);
		server.Start();
		var resultTask = server.GetContextAsync().ContinueWith(task =>
		{
			var context = task.Result;
			string content;
			using (var sr = new StreamReader(context.Request.InputStream, Encoding.UTF8))
				content = sr.ReadToEnd();

			context.Response.ContentLength64 = 0;
			context.Response.Close();
			return (
				Method: context.Request.HttpMethod,
				context.Request.ContentType,
				context.Request.Headers,
				Content: content
			);
		});

		string message = "Test message";
		var severity = RaygunSeverity.Critical;
		string[] tags = ["tag1", "tag2"];
		var httpClientFactory = new TestHttpClientFactory();
		var raygunOptions = new RaygunOptions
		{
			ApiKey = "API_KEY",
			ApiEndpoint = new(serverUrl),
			AppVersion = "1.0",
			ThrowOnError = true
		};
		var raygunMessageBuilder = new DefaultRaygunMessageBuilder(
		[
			new MainMessageProvider(Options.Create(raygunOptions)),
			new TestUserMessageProvider()
		]);
		var raygunClient = new DefaultRaygunClient(
			NullLoggerFactory.Instance.CreateLogger<DefaultRaygunClient>(),
			httpClientFactory,
			raygunMessageBuilder,
			[],
			Options.Create(raygunOptions)
		);
		await raygunClient.SendAsync(message, severity, tags);
		server.Stop();

		var result = await resultTask;
		result.Method.Should().Be("POST");
		result.Headers["X-ApiKey"].Should().Be(raygunOptions.ApiKey);
		result.Headers["Content-Length"].Should().NotBeNull();
		result.ContentType.Should().StartWith("application/json");
		result.Content.Should().NotBeNullOrEmpty();
		result.Content.Should().NotContain("\t");

		var resultMessage = JsonSerializer.Deserialize<RaygunMessage>(result.Content, new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		})!;
		resultMessage.Details.Version.Should().Be(raygunOptions.AppVersion);
		resultMessage.Details.Tags.Should().BeEquivalentTo(tags.Prepend(severity.ToString()));
		resultMessage.Details.Error.Message.Should().Be(message);
		resultMessage.Details.User?.Identifier.Should().Be(TestUserMessageProvider.Email);
		resultMessage.Details.User?.Email.Should().Be(TestUserMessageProvider.Email);
		resultMessage.Details.User?.IsAnonymous.Should().BeFalse();
	}
}