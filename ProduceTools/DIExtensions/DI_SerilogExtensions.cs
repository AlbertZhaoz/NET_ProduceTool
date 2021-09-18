using Albert.Extensions;
using Albert.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DI_SerilogExtensions
    {
        public static void AddSerilogExtensions(this IServiceCollection service)
        {
            service.AddScoped<ISeriLog, SerilogExtension>();
        }
    }
}
