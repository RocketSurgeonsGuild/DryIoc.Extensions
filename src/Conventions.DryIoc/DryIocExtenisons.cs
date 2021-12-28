using DryIoc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.DryIoc;

/// <summary>
/// Extension methods for dryioc
/// </summary>
public static class DryIocExtenisons
{
    /// <summary>
    /// Applies conventions from the given context onto the container
    /// </summary>
    /// <param name="container"></param>
    /// <param name="conventionContext"></param>
    /// <param name="services"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static IContainer ApplyConventions(this IContainer container, IConventionContext conventionContext, IServiceCollection services)
    {
        var configuration = conventionContext.Get<IConfiguration>() ?? throw new ArgumentException("Configuration was not found in context");
        foreach (var item in conventionContext.Conventions.Get<IDryIocConvention, DryIocConvention>())
        {
            if (item is IDryIocConvention convention)
            {
                container = convention.Register(conventionContext, configuration, services, container);
            }
            else if (item is DryIocConvention @delegate)
            {
                container = @delegate(conventionContext, configuration, services, container);
            }
        }

        return container;
    }
}
