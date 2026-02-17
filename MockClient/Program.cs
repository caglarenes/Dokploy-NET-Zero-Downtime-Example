using System.Net;

namespace Client;

internal class Program
{
    private static readonly Uri PROVIDER_URI = new(Environment.GetEnvironmentVariable("SERVER_ADDRESS"));

    private static readonly SocketsHttpHandler SOCKETS_HANDLER = new()
    {
        AutomaticDecompression = DecompressionMethods.All,
        PooledConnectionLifetime = TimeSpan.FromMinutes(15),
        KeepAlivePingDelay = TimeSpan.FromSeconds(60),
        PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
        EnableMultipleHttp2Connections = true,
        SslOptions = new System.Net.Security.SslClientAuthenticationOptions
        {
            RemoteCertificateValidationCallback = (_, _, _, _) => true
        }
    };

    private static readonly HttpClient HTTP_CLIENT = new(SOCKETS_HANDLER)
    {
        BaseAddress = PROVIDER_URI,
        DefaultRequestVersion = HttpVersion.Version20,
        DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact,
        Timeout = TimeSpan.FromSeconds(45)
    };

    static async Task Main(string[] args)
    {
        Console.WriteLine("Mock Client Starting...");

        _ = Task.Factory.StartNew(async () =>
        {
            while (true)
            {
                _ = SendRequest();
                await Task.Delay(1000);
            }
        }, TaskCreationOptions.LongRunning);

        await Task.Delay(-1);
    }

    public static async Task SendRequest()
    {
        try
        {
            Console.WriteLine(await HTTP_CLIENT.GetStringAsync("/"));
        }
        catch (Exception e)
        {
            Console.WriteLine($"HTTP Client Error: {e}");
        }
    }
}
