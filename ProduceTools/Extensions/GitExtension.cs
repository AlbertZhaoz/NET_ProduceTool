using Albert.Interface;
using Albert.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.IO;

namespace Albert.Extensions
{
    public class GitExtension: IGit
    {
        private string Src { get; set; }
        private readonly IOptionsSnapshot<ProduceToolEntity> options;
        private readonly ILogger<GitExtension> loggers;

        public GitExtension(IOptionsSnapshot<ProduceToolEntity> options, ILogger<GitExtension> loggers)
        {
            this.options = options;
            this.loggers = loggers;
            this.Src = options.Value.Repo.DefaultPath;
        }
        public void ChangeSrc(string newPath) => this.Src = newPath;     
        public void OpenInput(string cmd)=> GitCommand.GitCommandExcute(this.Src, cmd);
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
        public void ProduceNetCore()=> GitCommand.GitCommandExcute(this.Src, "produce netcore");

        public void RunGitExtensions(IServiceCollection service,string[] args)
        {
            using (var sp = service.BuildServiceProvider())
            {
                ///执行简化流程的Git:cd ..;git add .;git commit -m xxx;git push
                if ((!string.IsNullOrEmpty(args[0])) && args[0].Contains("git"))
                {
                    if (!string.IsNullOrEmpty(args[1]))
                    {
                        var gitExtensions = sp.GetRequiredService<IGit>();
                        gitExtensions.OpenInput("cd ..");
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
        public static void GitCommandExcute(string path, string command)
        {
            using (Process process = new Process())
            {
                //Process对象如果是exe必须设置为false
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WorkingDirectory = path;
                process.StartInfo.FileName = Path.Combine(Environment.SystemDirectory, "cmd.exe");
                //标准重定向流
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                //抓取事件
                process.OutputDataReceived += new DataReceivedEventHandler(OutputEventHandler);
                process.ErrorDataReceived += new DataReceivedEventHandler(ErrorEventHandler);
                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.StandardInput.WriteLine(command);
                process.StandardInput.WriteLine("exit");
                process.WaitForExit();
            }
        }
        private static void OutputEventHandler(Object sender, DataReceivedEventArgs e) => Console.WriteLine(e.Data);
        private static void ErrorEventHandler(Object sender, DataReceivedEventArgs e) => Console.WriteLine(e.Data);
    }
}
