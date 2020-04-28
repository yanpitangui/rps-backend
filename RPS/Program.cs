using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using RPS.Extensions;
using Serilog;
using System;

namespace RPS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = SerilogExtension.CreateLogger();
            try
            {
                Log.Logger.Information("Application starting up...");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Logger.Fatal(ex, "Application startup failed.");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://192.168.1.10:5001", "http://localhost:5001");
                });
    }
}
