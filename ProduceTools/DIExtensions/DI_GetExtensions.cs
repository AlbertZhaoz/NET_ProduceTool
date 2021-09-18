using Albert.Interface;
using ProduceTools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DI_GetExtensions
    {
        public static void AddSerilogExtensions(this IServiceCollection service)
        {
            service.AddScoped<IGit, GitExtension>();
        }
    }
}
