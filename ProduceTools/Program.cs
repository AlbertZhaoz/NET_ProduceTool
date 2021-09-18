﻿using System.Xml.Linq;
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
        private static ServiceCollection service = new ServiceCollection();
        /// <summary>
        /// <para>初始化DI:<see cref="InitService"/>
        /// </para>
        /// <para>Git简化:<see cref="AlbertGitExtensions"/>
        /// </para>
        /// <para>常规网站爬虫：<see cref="AlbertSimpleCrawl"/>
        /// </para>
        /// <para>Azure云API分析：<see cref="AlbertAzureExtensions"/>
        /// </para>
        /// </summary>
        /// <param name="args"></param>
        /// <remarks>目前预计支持四类工具，1.是简化git流程，不需要整一大堆指令;
        /// 2.常规Produce流程：在readme.md文件中已描述；
        /// 3.常规网站的爬虫程序；
        /// 4.Azure云反爬虫的爬虫程序</remarks>
        static void Main(string[] args)
        {
            InitService();
            AlbertGitExtensions(args);
        }

        static void InitService()
        {
            service.AddScoped<GitExtension>();
            service.AddScoped<SimpleCrawlerExtension>();

            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("Settings\\ProduceTool.Json", false, true);
            var rootConfig = configurationBuilder.Build();

            service.AddOptions().Configure<ProduceToolEntity>(e => rootConfig.Bind(e))
                .Configure<Repo>(e => rootConfig.GetSection("Repo").Bind(e))
                .Configure<MsBuild>(e => rootConfig.GetSection("MsBuild").Bind(e))
                .Configure<AzureDevOps>(e => rootConfig.GetSection("AzureDevOps").Bind(e))
                .Configure<PersonalCrawling>(e => rootConfig.GetSection("PersonalCrawling").Bind(e));
        }

        static void AlbertGitExtensions(string[] args)
        {
            using (var sp = service.BuildServiceProvider())
            {
                ///执行简化流程的Git:cd ..;git add .;git commit -m xxx;git push
                if ((!string.IsNullOrEmpty(args[0])) && args[0].Contains("albertgit"))
                {
                    if (!string.IsNullOrEmpty(args[1]))
                    {
                        var gitExtensions = sp.GetRequiredService<GitExtension>();
                        gitExtensions.OpenInput("cd ..");
                        gitExtensions.GitAdd();
                        string comment = args[1].Split(" ").Last();
                        gitExtensions.Commit(comment);
                        gitExtensions.Push();
                        Console.WriteLine("Run Successfully!");
                    }
                    else Console.WriteLine("Please input some comments.");
                }             
            }
        }

        static void AlbertProduce()

        static void AlbertSimpleCrawl(string[] args)
        {
            using (var sp = service.BuildServiceProvider())
            {
                ///普通网站的爬虫，重定向问题需要单独配置相关的设置
                if ((!string.IsNullOrEmpty(args[0])) && args[0].Contains("albertcrawl"))
                {
                    if (!string.IsNullOrEmpty(args[1]))
                    {
                        var simpleCrawlerExtension = sp.GetRequiredService<SimpleCrawlerExtension>();
                        simpleCrawlerExtension.OnStart += (s, e) =>
                        {
                            Console.WriteLine("爬虫开始抓取地址：" + e.Uri.ToString());
                        };
                        simpleCrawlerExtension.OnError += (s, e) =>
                        {
                            Console.WriteLine("爬虫抓取出现错误：" + e.Uri.ToString() + "，异常消息：" + e.Exception.Message);
                        };
                        simpleCrawlerExtension.OnCompleted += (s, e) =>
                        {
                            //使用正则表达式清洗网页源代码中的数据
                            var links = Regex.Matches(e.PageSource, @"<a[^>]+href=""*(?<href>/hotel/[^>\s]+)""\s*[^>]*>(?<text>(?!.*img).*?)</a>", RegexOptions.IgnoreCase);
                            foreach (Match match in links)
                            {
                                var city = new City
                                {
                                    CityName = match.Groups["text"].Value,
                                    Uri = new Uri("http://hotels.ctrip.com" + match.Groups["href"].Value
                                )
                                };
                                if (!cityList.Contains(city)) cityList.Add(city);//将数据加入到泛型列表
                                Console.WriteLine(city.CityName + "|" + city.Uri);//将城市名称及URL显示到控制台
                            }
                            Console.WriteLine("===============================================");
                            Console.WriteLine("爬虫抓取任务完成！合计 " + links.Count + " 个城市。");
                            Console.WriteLine("耗时：" + e.Milliseconds + "毫秒");
                            Console.WriteLine("线程：" + e.ThreadId);
                            Console.WriteLine("地址：" + e.Uri.ToString());
                        };
                        cityCrawler.Start(new Uri(cityUrl)).Wait();//没被封锁就别使用代理：60.221.50.118:8090
                        Console.WriteLine("Run Successfully!");
                    }
                    else Console.WriteLine("Please input some comments.");
                }
            }
        }

        static void AlbertAzureExtensions(string[] args)
        {

        }
    }
}
