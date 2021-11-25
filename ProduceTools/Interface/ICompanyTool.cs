using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Albert.Interface
{
    internal interface ICompanyTool
    {
        void RunCompanyToolExtensions(IServiceProvider sp, string[] args);
    }
}
