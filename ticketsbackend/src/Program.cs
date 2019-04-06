using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace ticketsbackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).UseKestrel(options => options.AddServerHeader = false)
                .UseStartup<Startup>().UseUrls("http://+:5000") 
                .ConfigureAppConfiguration((builderContext, config) => { config.AddEnvironmentVariables(); });

    }
}

