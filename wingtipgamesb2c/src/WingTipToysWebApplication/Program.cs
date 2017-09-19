using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace WingTipToysWebApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webHost = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            webHost.Run();
        }
    }
}
