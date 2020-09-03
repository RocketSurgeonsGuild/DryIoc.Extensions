using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Extensions.DryIoc.Tests;

namespace Rocket.Surgery.Extensions.DryIoc.Tests
{
    internal class LoggingBuilder : ILoggingBuilder
    {
        public LoggingBuilder(IServiceCollection services) => Services = services;

        public IServiceCollection Services { get; }
    }
}