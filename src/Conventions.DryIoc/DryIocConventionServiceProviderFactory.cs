using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Rocket.Surgery.Conventions.DryIoc
{
    class DryIocConventionServiceProviderFactory : IServiceProviderFactory<IContainer>
    {
        private readonly IConventionContext _conventionContext;
        private readonly IContainer _container;

        public DryIocConventionServiceProviderFactory(IConventionContext conventionContext, IContainer? container = null)
        {
            _conventionContext = conventionContext;
            _container = container ?? new Container().WithDependencyInjectionAdapter();
        }

        public IContainer CreateBuilder(IServiceCollection services)
        {
            var container = _container;

            var configuration = _conventionContext.Get<IConfiguration>() ?? throw new ArgumentException("Configuration was not found in context");
            foreach (var item in _conventionContext.Conventions.Get<IDryIocConvention, DryIocConvention>())
            {
                if (item.Convention is IDryIocConvention convention)
                {
                    container = convention.Register(_conventionContext, configuration, services, container);
                }
                else if (item.Delegate is DryIocConvention @delegate)
                {
                    container = @delegate(_conventionContext, configuration, services, container);
                }
            }

            container.Populate(services);

            return container;
        }

        public IServiceProvider CreateServiceProvider(IContainer containerBuilder) => containerBuilder;
    }
}