using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Albert.Interface
{
    public interface IGit
    {
        void ChangeSelfWorkPath();
        void ChangeSrc(string newPath);
        void OpenInput(string cmd);
        void GitAdd();
        void GetGitVersion();
        void Clone(string repo);
        void Chekcout(string branch);
        void NewBranch(string branchName);
        void Pull();
        void Commit();
        void Commit(string commitNote);
        void Stash();
        void Push();
        void Push(string branchName);
        void DeleteBranch(string branchName);
        void ProduceNetCore();
        void RunGitExtensions(IServiceProvider sp, string[] args);
    }
}
