using Albert.Interface;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProduceTools.Extensions
{
    public class GitExtension: IGit
    {
        private string Src { get; set; }
        private readonly IOptionsSnapshot<ProduceToolEntity> options;

        public GitExtension(IOptionsSnapshot<ProduceToolEntity> options)
        {
            this.options = options;
            this.Src = options.Value.Repo.DefaultPath;
        }
        public void ChangeSrc(string newPath) => this.Src = newPath;     
        public void OpenInput(string cmd)=> GitCommand.GitCommandExcute(this.Src, cmd);
        public void GitAdd() => GitCommand.GitCommandExcute(this.Src, $"git add .");
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
