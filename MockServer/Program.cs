
namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var appVersion = $"App Version: {Random.Shared.Next(10_000, 100_000)}";
            Console.WriteLine(appVersion);

            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Limits.Http2.KeepAlivePingDelay = TimeSpan.FromSeconds(30);
                options.Limits.Http2.KeepAlivePingTimeout = TimeSpan.FromSeconds(10);
            });

            builder.Services.Configure<HostOptions>(options =>
            {
                options.ShutdownTimeout = TimeSpan.FromSeconds(60);
            });

            builder.Services.AddHealthChecks();

            var app = builder.Build();

            app.MapGet("/", async () =>
            {
                await Task.Delay(15000);
                return Results.Text(appVersion);
            });

            app.MapHealthChecks("/health");

            app.Use(async (context, next) =>
            {
                Console.WriteLine($"{context.Request.Method} {context.Request.Path} - IP: {context.Connection.RemoteIpAddress?.ToString()} - HTTP Version: {context.Request.Protocol}");
                await next();
            });

            app.Run();
        }
    }
}
