using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using Serilog;
using Serilog.Formatting.Json;
using Albert.Interface;
using Exceptionless;
using Albert.Model;
using System.Data.SqlClient;

namespace Albert
{
    class Program
    {
        private static ServiceCollection service = new ServiceCollection();

        /// <summary>
        /// <para>初始化DI:<see cref="InitService"/>
        /// </para>
        /// <para>Git拓展:<see cref="Extensions.GitExtension.RunGitExtensions"/>
        /// </para>
        /// <para>Produce自动化:<see cref="Extensions.ProduceExtension.RunProduceExtensions"/>
        /// </para>
        /// <para>常规网站爬虫：<see cref="Extensions.SimpleCrawlerExtension.RunSimpleCrawlerExtension"/>
        /// </para>
        /// <para>Azure云API分析：<see cref="Extensions.AzureDevOpsExtension.RunAzureDevOpsExtension"/>
        /// </para>
        /// </summary>
        /// <param name="args"></param>
        /// <remarks>
        /// 目前预计支持四类工具:
        /// 1.Git拓展：简化git流程，不需要整一大堆指令;
        /// 2.常规Produce流程：在readme.md文件中已描述；
        /// 3.常规网站的爬虫程序；
        /// 4.Azure云反爬虫的爬虫程序
        /// </remarks>
        static void Main(string[] args)
        {
            InitService();
            using (var sp = service.BuildServiceProvider())
            {
                sp.GetRequiredService<IGit>().RunGitExtensions(sp, args);
                sp.GetRequiredService<ICrawler>().RunSimpleCrawlerExtension(sp, args);
                sp.GetRequiredService<IHelper>().RunHelperInfoExtension(sp, args);
            }
        }

        static void InitService()
        {
            service.AddGitExtensions();
            service.AddSimpleCrawlerExtensions();
            service.AddSerilogExtensions();
            service.AddGetHelperExtensions();

            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            //从sqlserver数据库中获取数据，暂时先手写连接字符串,设置超时时间从默认15s变为5s
            try
            {
                string strConfigFromSqlserver = "Server = .; Database = AlbertConfigDb; Trusted_Connection = True;MultipleActiveResultSets=true;Connect Timeout=500";
                configurationBuilder.AddDbConfiguration(() => new SqlConnection(strConfigFromSqlserver),
                    reloadOnChange: true,
                    reloadInterval: TimeSpan.FromSeconds(2),
                    tableName: "ProduceToolConfig");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }           
            configurationBuilder.AddUserSecrets<Program>();//防止机密信息上传到Github
            configurationBuilder.AddJsonFile("Configs\\ProduceTool.Json", false, true);           
            var rootConfig = configurationBuilder.Build();

            service.AddOptions().Configure<ProduceToolEntity>(e => rootConfig.Bind(e))
                .Configure<Repo>(e => rootConfig.GetSection("Repo").Bind(e))
                .Configure<MsBuild>(e => rootConfig.GetSection("MsBuild").Bind(e))
                .Configure<AzureDevOps>(e => rootConfig.GetSection("AzureDevOps").Bind(e))
                .Configure<PersonalCrawling>(e => rootConfig.GetSection("PersonalCrawling").Bind(e))
                .Configure<HelperInfo>(e=>rootConfig.GetSection("HelperInfo").Bind(e));
            //ToDo:Serilog Write Information to File
            using (var sp = service.BuildServiceProvider())
            {
                var serilogExtension = sp.GetRequiredService<ISeriLog>();
                if (serilogExtension.OpenExceptionlessClient())
                {
                    //配置ExceptionlessClient启动密钥,从UserSecrets-ProduceTool.Json获取
                    ExceptionlessClient.Default.Startup(serilogExtension.ExceptionlessClientDefaultStartUpKey);
                    ExceptionlessClient.Default.Configuration.SetDefaultMinLogLevel(Exceptionless.Logging.LogLevel.Trace);
                    service.AddLogging(e => {
                        Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
                        .Enrich.FromLogContext()
                        .WriteTo.Console(new JsonFormatter())
                        .WriteTo.Exceptionless()
                        .CreateLogger();
                        e.AddSerilog();
                    });
                }
                else
                {
                    service.AddLogging(e => {
                        Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
                        .Enrich.FromLogContext()
                        .WriteTo.Console(new JsonFormatter())
                        .CreateLogger();
                        e.AddSerilog();
                    });
                }
            }            
        }   
    }
}
