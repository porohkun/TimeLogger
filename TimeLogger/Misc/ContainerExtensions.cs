using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TimeLogger.Attributes;

namespace TimeLogger.Misc
{
    public static class ContainerExtensions
    {
        public static void AddSingletonsFromAttributes(this IServiceCollection services)
        {
            AddFromAttributes<AsSingletoneAttribute>((s, i) => services.AddSingleton(s, i));
        }

        public static void AddTransientsFromAttributes(this IServiceCollection services)
        {
            AddFromAttributes<AsTransientAttribute>((s, i) => services.AddTransient(s, i));
        }

        private static void AddFromAttributes<TAttribute>(Action<Type, Type> register)
            where TAttribute : ContainerRegisterAttribute
        {
            foreach (var type in typeof(object).GetInheritors())
            {
                try
                {
                    var attribute = type.GetCustomAttribute<TAttribute>();

                    if (attribute == null)
                        continue;

                    if (attribute.Abstractions.Any())
                        foreach (var abstraction in attribute.Abstractions)
                            register(abstraction, type);
                    else
                        register(type, type);
                }
                catch (Exception ex)
                {
                    throw new Exception($"!!{type}:{ex.Message}", ex);
                }
            }
        }
    }
}