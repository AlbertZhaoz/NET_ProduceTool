using Albert.Extensions;
using Albert.Interface;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DI_GitExtensions
    {
        public static void AddGitExtensions(this IServiceCollection service)
        {
            service.AddScoped<IGit, GitExtension>();
        }
    }
}
