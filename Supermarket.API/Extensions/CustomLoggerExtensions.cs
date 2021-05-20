using Microsoft.Extensions.Logging;
using Supermarket.API.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.API.Logging
{
    public static class CustomLoggerExtensions
    {
        public static ILoggerFactory AddColoredConsoleLogger(this ILoggerFactory loggerFactory, CustomLoggerConfiguration config)
        {
            loggerFactory.AddProvider(new CustomLoggerProvider(config));
            return loggerFactory;
        }

        public static ILoggerFactory AddColoredConsoleLogger(this ILoggerFactory loggerFactory)
        {
            var config = new CustomLoggerConfiguration();
            return loggerFactory.AddColoredConsoleLogger(config);
        }

        public static ILoggerFactory AddColoredConsoleLogger(this ILoggerFactory loggerFactory, Action<CustomLoggerConfiguration> configure)
        {
            var config = new CustomLoggerConfiguration();
            configure(config);
            return loggerFactory.AddColoredConsoleLogger(config);
        }
    }
}
