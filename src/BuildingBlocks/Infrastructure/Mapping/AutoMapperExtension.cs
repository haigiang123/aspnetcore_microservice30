using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Mapping
{
    public static class AutoMapperExtension
    {
        public static IMappingExpression<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        {
            var flags = BindingFlags.Instance | BindingFlags.Public;

            var sourceType = typeof(TSource);
            var destinationType = typeof(TDestination).GetProperties(flags);

            foreach (var item in destinationType)
            {
                if(sourceType.GetProperty(item.Name, flags) == null)
                {
                    expression.ForMember(item.Name, x => x.Ignore());
                }
            }

            return expression;
        }
    }
}
