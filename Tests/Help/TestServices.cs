using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace SchoolAssistans.Tests
{
    internal static class TestServices
    {
        private static IServiceCollection _collection = null!;
        public static IServiceCollection Collection
        {
            get
            {
                if (_collection is null)
                {
                    _collection = new ServiceCollection();
                    _collection.AddTransient<ILoggerFactory, LoggerFactory>();
                }
                return _collection;
            }
        }

        public static IServiceProvider Provider => _collection.BuildServiceProvider();

        public static TService GetService<TService>()
            where TService : notnull
            => Provider.GetRequiredService<TService>();

        public static void AddService<TService>()
            where TService : class
            => Collection.AddTransient<TService>();

        public static void AddServiceAndLogger<TService>()
            where TService : class
        {
            Collection.AddTransient<ILogger<TService>, Logger<TService>>();
            Collection.AddTransient<TService>();
        }

        public static void AddService<TService, TServiceImplementation>()
            where TService : class
            where TServiceImplementation : class, TService
            => Collection.AddTransient<TService, TServiceImplementation>();

        public static void AddServiceAndLogger<TService, TServiceImplementation>()
            where TService : class
            where TServiceImplementation : class, TService
        {
            Collection.AddTransient<ILogger<TService>, Logger<TService>>();
            Collection.AddTransient<TService, TServiceImplementation>();
        }

        public static void Clear() => Collection.Clear();
    }
}
