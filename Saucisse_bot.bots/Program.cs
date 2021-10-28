using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Saucisse_bot.bots
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            //var bot = new Bot();
            //bot.RunAsync().GetAwaiter().GetResult();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
            
    }
}
