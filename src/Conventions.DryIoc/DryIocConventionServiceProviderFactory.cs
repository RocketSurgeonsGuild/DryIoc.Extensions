using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.DryIoc;

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
        var container = _container.ApplyConventions(_conventionContext, services);
        container.Populate(services);
        return container;
    }

    public IServiceProvider CreateServiceProvider(IContainer containerBuilder) => containerBuilder;
}
