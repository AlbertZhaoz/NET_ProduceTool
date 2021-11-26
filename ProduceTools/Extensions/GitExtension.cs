using Albert.Interface;
using Albert.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Albert.Extensions
{
    public class GitExtension : IGit
    {
        private string Src { get; set; }
        private readonly IOptionsSnapshot<ProduceToolEntity> options; //依赖注入可选项
        private readonly ILogger<GitExtension> loggers; //依赖注入日志服务

        public GitExtension(IOptionsSnapshot<ProduceToolEntity> options, ILogger<GitExtension> loggers)
        {
            this.options = options;
            this.loggers = loggers;
            this.Src = options.Value.Repo.DefaultPath;
        }
        public void ChangeSrc(string newPath)
        {
            if(newPath != default(string))
            {
                this.Src = newPath;

            }            
            GitCommand.GitCommandExcute(this.Src, "cd \\");
            GitCommand.GitCommandExcute(this.Src, $"cd this.Src");
        }
        public void OpenInput(string cmd) => GitCommand.GitCommandExcute(this.Src, cmd);
        public void GitAdd() => GitCommand.GitCommandExcute(this.Src, "git add .");
        public void GetGitVersion() => GitCommand.GitCommandExcute(this.Src, "git --version");
        public void Clone(string repo) => GitCommand.GitCommandExcute(this.Src, "git clone " + repo + " .");
        public void Chekcout(string branch) => GitCommand.GitCommandExcute(this.Src, $"git checkout {branch}");
        public void NewBranch(string branchName) => GitCommand.GitCommandExcute(this.Src, $"git checkout -b {branchName}");
        public void Pull() => GitCommand.GitCommandExcute(this.Src, "git pull");
        public void Commit() => GitCommand.GitCommandExcute(this.Src, "git commit -am update");
        public void Commit(string commitNote) => GitCommand.GitCommandExcute(this.Src, $"git commit -m \"{commitNote}\"");
        public void Stash() => GitCommand.GitCommandExcute(this.Src, "git stash");
        public void Push() => GitCommand.GitCommandExcute(this.Src, "git push");
        public void Push(string branchName) => GitCommand.GitCommandExcute(this.Src, $"git push --set-upstream origin {branchName}");
        public void DeleteBranch(string branchName)
        {
            GitCommand.GitCommandExcute(this.Src, $"git branch -D {branchName}");
            GitCommand.GitCommandExcute(this.Src, $"git push origin --delete {branchName}");
        }
        public void ProduceNetCore() => GitCommand.GitCommandExcute(this.Src, "produce netcore");

        public void RunGitExtensions(IServiceProvider sp, string[] args)
        {
            //进入自定义git分支
            //执行简化流程的Git:cd \;cd repo;git add .;git commit -m xxx;git push
            //支持albert git "commit comments" albert git repopath "commit comments"
            var argsStr = string.Join(" ",args);
            if ((args.Length>0) && args[0].Contains("git"))
            {
                //这个分支是albert git repopath "commit comments"
                //更改仓库目录，并直接推送到远端
                if (args.Length > 2)
                {
                    if (!string.IsNullOrEmpty(args[1]))
                    {
                        if (!string.IsNullOrEmpty(args[2]))
                        {
                            var gitExtensions = sp.GetRequiredService<IGit>();
                            gitExtensions.ChangeSrc(args[1]);                        
                            gitExtensions.GitAdd();
                            string comment = args[2];
                            gitExtensions.Commit(comment);
                            gitExtensions.Push();
                            Console.WriteLine("Run Successfully!");
                            loggers.LogInformation("Run Successfully!");
                        }
                        else
                        {
                            Console.WriteLine("Please input your comments.");
                            loggers.LogInformation("Please input some comments like:albert git \"repo path\" \"comments\"");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Please input your repo path.");
                        loggers.LogInformation("Please input some comments like:albert git \"repo path\" \"comments\"");
                    }
                }
                //这个分支是albert git "commit comments" 简化一堆命令
                else
                {
                    if (!string.IsNullOrEmpty(args[1]))
                    {
                        var gitExtensions = sp.GetRequiredService<IGit>();
                        gitExtensions.ChangeSrc(default(string));
                        gitExtensions.GitAdd();
                        string comment = args[1];
                        gitExtensions.Commit(comment);
                        gitExtensions.Push();
                        Console.WriteLine("Run Successfully!");
                        loggers.LogInformation("Run Successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Please input some comments.");
                        loggers.LogInformation("Please input some comments like:albert git \"modify some files\"");
                    }
                }               
            }
        }
    }

    public static class GitCommand
    {
        //Fix everycommand execute exit command.
        //批量执行最后一步退出方法在CompanyToolExtensions中已实现
        public static void GitCommandExcute(string path, string command)
        {
            using (Process process = new Process())
            {
                //Process对象如果是exe必须设置为false
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WorkingDirectory = path;           
                process.StartInfo.FileName = Path.Combine(Environment.SystemDirectory, "cmd.exe");
                //标准重定向流
                Console.WriteLine(process.StartInfo.FileName);
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                //抓取事件
                process.OutputDataReceived += new DataReceivedEventHandler(OutputEventHandler);
                process.ErrorDataReceived += new DataReceivedEventHandler(ErrorEventHandler);
                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.StandardInput.WriteLine("\"%ProgramFiles(x86)%\\Microsoft Visual Studio\\2022\\Preview\\Common7\\Tools\\VsDevCmd.bat\"");
                process.StandardInput.WriteLine(command + "&exit");
                process.StandardInput.AutoFlush = true;
                process.WaitForExit();
                process.Close();
            }
        }
        private static void OutputEventHandler(Object sender, DataReceivedEventArgs e) => Console.WriteLine(e.Data);
        private static void ErrorEventHandler(Object sender, DataReceivedEventArgs e) => Console.WriteLine(e.Data);
    }
}
