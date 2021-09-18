using System.Xml.Linq;
using System.Net.Http;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using ProduceTools.Extensions;
using Microsoft.Extensions.Configuration;

namespace ProduceTools
{
    class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <remarks>目前预计支持两类工具，一种是简化git流程，不需要整一大堆指令;
        /// 另一种是常规Produce流程：在readme.md文件中已描述</remarks>
        static void Main(string[] args)
        {
            Console.WriteLine(args);
            var service = new ServiceCollection();
            service.AddScoped<GitExtension>();

            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("Settings\\ProduceTool.Json", false, true);
            var rootConfig = configurationBuilder.Build();

            service.AddOptions().Configure<ProduceToolEntity>(e => rootConfig.Bind(e))
                .Configure<Git>(e => rootConfig.GetSection("Git").Bind(e))
                .Configure<MsBuild>(e => rootConfig.GetSection("MsBuild").Bind(e))
                .Configure<WebClient>(e => rootConfig.GetSection("WebClient").Bind(e));

            using(var sp = service.BuildServiceProvider())
            {
                ///执行简化流程的Git:cd ..;git add .;git commit -m xxx;git push
                if (args[0].Contains("albertgit"))
                {
                    var gitExtensions = sp.GetRequiredService<GitExtension>();
                    gitExtensions.OpenInput("cd ..");
                    gitExtensions.GitAdd();
                    //解析args[0]
                    string comment = args[0].Split(" ").Last();
                    gitExtensions.Commit(comment);
                    gitExtensions.Push();
                }                     
            }         
        }
    }
}
