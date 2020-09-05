using DryIoc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.DryIoc
{
    /// <summary>
    /// Delegate ServiceConventionAction
    /// </summary>
    /// <param name="conventionContext"></param>
    /// <param name="configuration"></param>
    /// <param name="services"></param>
    /// <param name="container"></param>
    public delegate IContainer DryIocConvention(IConventionContext conventionContext, IConfiguration configuration, IServiceCollection services, IContainer container);
}