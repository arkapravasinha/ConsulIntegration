using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Winton.Extensions.Configuration.Consul;

namespace ConsulIntegration.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((context, cfgBuilder) =>
                {
                    cfgBuilder.AddEnvironmentVariables();
                    var environment = context.HostingEnvironment.EnvironmentName;
                    string key = Environment.GetEnvironmentVariable("ApplicationName")+"/appsettings." + environment + ".json";
                    cfgBuilder.AddConsul(key, options =>
                    {
                        //Configure Consul Connection Details, i.e. Address, DataCenter, Certificates and Auth details
                        options.ConsulConfigurationOptions =
                                        cco => { cco.Address = new Uri(Environment.GetEnvironmentVariable("ConsulURL"));};
                        //Making Configuration either optional or not
                        options.Optional = true;
                        //Wait Time before pulling an change from Consul
                        options.PollWaitTime = TimeSpan.FromSeconds(5);
                        //Whether Reload the Configuration if any changes are detected
                        options.ReloadOnChange = true;
                        //What action to perform if On Load Fails
                        options.OnLoadException = (consulLoadExceptionContext) =>
                        {
                            Console.WriteLine($"Error onLoadException {consulLoadExceptionContext.Exception.Message} and stacktrace {consulLoadExceptionContext.Exception.StackTrace}");
                            throw consulLoadExceptionContext.Exception;
                        };
                        //What action to perform if Watching Changes failed
                        options.OnWatchException = (consulWatchExceptionContext) =>
                        {
                            Console.WriteLine($"Unable to watchChanges in Consul due to {consulWatchExceptionContext.Exception.Message}");
                            return TimeSpan.FromSeconds(2);
                        };
                    });
                });
    }
}
