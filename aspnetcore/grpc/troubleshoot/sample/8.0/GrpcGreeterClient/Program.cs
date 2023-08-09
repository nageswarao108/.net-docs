﻿#define StandardHTTPS  // Options: StandardHTTPS | IgnoreInvalidCertificate | IgnoreInvalidCertificateClientFactory | CallInsecureGrpcServices | DotNet3InsecureGrpcServices | SubdirectoryHandler

#if StandardHTTPS
using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcGreeterClient;

// The port number must match the port of the gRPC server.
#region StandardHTTPS
using var channel = GrpcChannel.ForAddress("https://localhost:7106");
var client = new Greeter.GreeterClient(channel);
#endregion
var reply = await client.SayHelloAsync(
                  new HelloRequest { Name = "GreeterClient" });
Console.WriteLine("Greeting: " + reply.Message);
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
#endif

#if IgnoreInvalidCertificate
// Warning: Untrusted certificates should only be used during app development. 
// Production apps should always use valid certificates.
// The following gRPC client factory allows calls without a trusted certificate.

using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcGreeterClient;

#region IgnoreInvalidCertificate
var handler = new HttpClientHandler();
handler.ServerCertificateCustomValidationCallback = 
    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

using var channel = GrpcChannel.ForAddress("https://localhost:7106",
    new GrpcChannelOptions { HttpHandler = handler });
var client = new Greeter.GreeterClient(channel);
#endregion

var reply = await client.SayHelloAsync(
                  new HelloRequest { Name = "GreeterClient" });
Console.WriteLine("Greeting: " + reply.Message);
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
#endif

#if IgnoreInvalidCertificateClientFactory
// Warning: Untrusted certificates should only be used during app development. 
// Production apps should always use valid certificates.
// The following gRPC client factory allows calls without a trusted certificate.

using GrpcGreeterClient;
using Microsoft.Extensions.DependencyInjection;
using Grpc.Net.ClientFactory;

#region IgnoreInvalidCertificateClientFactory

var services = new ServiceCollection();

services
    .AddGrpcClient<Greeter.GreeterClient>(o =>
    {
        o.Address = new Uri("https://localhost:5001");
    })
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        return handler;
    });

#endregion
#endif


#if CallInsecureGrpcServices
using Grpc.Net.Client;
using GrpcGreeterClient;

#region CallInsecureGrpcServices
AppContext.SetSwitch(
    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

using var channel = GrpcChannel.ForAddress("http://localhost:5186");
var client = new Greeter.GreeterClient(channel);
#endregion
var reply = await client.SayHelloAsync(
                  new HelloRequest { Name = "GreeterClient" });
Console.WriteLine("Greeting: " + reply.Message);
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
#endif

#if DotNet3InsecureGrpcServices
// Warning: Untrusted certificates should only be used during app development. 
// Production apps should always use valid certificates.
// The following gRPC client factory allows calls without a trusted certificate.

using Grpc.Net.Client;
using GrpcGreeterClient;

#region DotNet3InsecureGrpcServices
var handler = new HttpClientHandler();
handler.ServerCertificateCustomValidationCallback = 
    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

using var channel = GrpcChannel.ForAddress("https://localhost:7106",
    new GrpcChannelOptions { HttpHandler = handler });
var client = new Greeter.GreeterClient(channel);
#endregion

var reply = await client.SayHelloAsync(
                  new HelloRequest { Name = "GreeterClient" });
Console.WriteLine("Greeting: " + reply.Message);
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
#endif


#if SubdirectoryHandler
using Grpc.Net.Client;
using GrpcGreeterClient;

#region CallSubdirectoryHandler
var handler = new SubdirectoryHandler(new HttpClientHandler(), "/MyApp");

using var channel = GrpcChannel.ForAddress("https://localhost:7106", new GrpcChannelOptions { HttpHandler = handler });
var client = new Greeter.GreeterClient(channel);

var reply = await client.SayHelloAsync(
                  new HelloRequest { Name = "GreeterClient" });
Console.WriteLine("Greeting: " + reply.Message);
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
#endregion

#region SubdirectoryHandler
/// <summary>
/// A delegating handler that adds a subdirectory to the URI of gRPC requests.
/// </summary>
public class SubdirectoryHandler : DelegatingHandler
{
    private readonly string _subdirectory;

    public SubdirectoryHandler(HttpMessageHandler innerHandler, string subdirectory)
        : base(innerHandler)
    {
        _subdirectory = subdirectory;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var old = request.RequestUri;

        var url = $"{old.Scheme}://{old.Host}:{old.Port}";
        url += $"{_subdirectory}{request.RequestUri.AbsolutePath}";
        request.RequestUri = new Uri(url, UriKind.Absolute);

        return base.SendAsync(request, cancellationToken);
    }
}
#endregion
#endif

#if Http3Handler
using Grpc.Net.Client;
using GrpcGreeterClient;
using System.Net;
#region CallHttp3Handler
var handler = new Http3Handler(new HttpClientHandler());

var channel = GrpcChannel.ForAddress("https://localhost:7106", new GrpcChannelOptions { HttpHandler = handler });
var client = new Greeter.GreeterClient(channel);

var reply = await client.SayHelloAsync(
                  new HelloRequest { Name = "GreeterClient" });
Console.WriteLine("Greeting: " + reply.Message);
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
#endregion

#region Http3Handler
/// <summary>
/// A delegating handler that changes the request HTTP version to HTTP/3.
/// </summary>
public class Http3Handler : DelegatingHandler
{
    public Http3Handler() { }
    public Http3Handler(HttpMessageHandler innerHandler) : base(innerHandler) { }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Version = HttpVersion.Version30;
        request.VersionPolicy = HttpVersionPolicy.RequestVersionExact;

        return base.SendAsync(request, cancellationToken);
    }
}
#endregion
#endif