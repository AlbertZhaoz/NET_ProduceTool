using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProduceTools.Extensions
{
    public class GitExtension
    {
        public GitExtension(string src) => this.Src = src;

        private string Src { get; set; }

        public 
        public void Clone(string repo) => GitCommand.GitCommandExcute(this.Src, "git clone " + repo + " .");
        public void Chekcout(string branch) => GitCommand.GitCommandExcute(this.Src, "git checkout " + branch);
        public void NewBranch(string branchName) => GitCommand.GitCommandExcute(this.Src, "git checkout -b " + branchName);
        public void Pull() => GitCommand.GitCommandExcute(this.Src, "git pull");
        public void Commit() => GitCommand.GitCommandExcute(this.Src, "git commit -am update");
        public void Stash() => GitCommand.GitCommandExcute(this.Src, "git stash");
        public void Push(string branchName) => GitCommand.GitCommandExcute(this.Src, "git push --set-upstream origin " + branchName);
        public void DeleteBranch(string branchName)
        {
            GitCommand.GitCommandExcute(this.Src, "git branch -D " + branchName);
            GitCommand.GitCommandExcute(this.Src, "git push origin --delete " + branchName);
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
                //
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
