using DryIoc;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DryIoc;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Class DryIocRocketHostExtensions.
/// </summary>
[PublicAPI]
public static class DryIocRocketHostExtensions
{
    /// <summary>
    /// Uses the DryIoc.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="container">The container.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder UseDryIoc(this ConventionContextBuilder builder, IContainer? container = null)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.ConfigureHosting((context, builder) => builder.UseServiceProviderFactory(new DryIocConventionServiceProviderFactory(context, container)));
    }
}
