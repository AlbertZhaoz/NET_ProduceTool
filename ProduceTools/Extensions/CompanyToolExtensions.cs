using Albert.Interface;
using Albert.Model;
using Albert.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Albert.Extensions
{
    internal class CompanyToolExtensions:ICompanyTool
    {
        private string CompanyToolEnlistmentPath { get; set; }
        private readonly IOptionsSnapshot<ProduceToolEntity> options; //依赖注入可选项
        private readonly ILogger<GitExtension> loggers; //依赖注入日志服务

        public CompanyToolExtensions(IOptionsSnapshot<ProduceToolEntity> options, ILogger<GitExtension> loggers)
        {
            this.options = options;
            this.loggers = loggers;
            this.CompanyToolEnlistmentPath = options.Value.Repo.DefaultPath + Regex.Replace(options.Value.Repo.CompanyToolEnlistmentPath, "", options.Value.Repo.DefaultPath, RegexOptions.IgnoreCase);
        }

        public void RunCompanyToolExtensions(IServiceProvider sp, string[] args)
        {
            var argsStr = string.Join(" ", args);
            if ((args.Length > 0) && args[0].Contains("company"))
            {
                var companyToolExtensions = sp.GetRequiredService<ICompanyTool>();
                List<string> cmdList = new List<string>()
            {
                "D:\\repo\\src\\tools\\path1st\\myenv.cmd",
                "cd sources/dev/Hygiene/src/DataInsights/Kql.NetCore",
                "msbuild -t:restore",
                "gdeps -f",
                "cd \\",
                "dir"
            };
                Command.ExecuteCmd(cmdList);
            }
            else
            {
                //loggers.LogInformation("Please input some comments like:albert company \"modify some files\"");
            }
        }
    }
}
