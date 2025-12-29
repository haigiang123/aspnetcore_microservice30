using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    public static class ConfigurationExtensions
    {
        public static T GetOptions<T>(this IServiceCollection services, string sectionName)
            where T : new ()
        {
            using (var serviceProvider = services.BuildServiceProvider())
            {
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                var section = config.GetSection(sectionName);
                var result = new T();
                section.Bind(result);

                return result;
            }
        }
    }
}
