using DryIoc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions.DryIoc;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions;

/// <summary>
/// Class DryIocRocketHostExtensions.
/// </summary>
[PublicAPI]
public static class DryIocConventionRocketHostExtensions
{
    /// <summary>
    /// Uses the DryIoc.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="delegate">The container.</param>
    /// <returns>IHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureDryIoc(this ConventionContextBuilder builder, DryIocConvention @delegate)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.AppendDelegate(@delegate);
        return builder;
    }

    /// <summary>
    /// Uses the DryIoc.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="delegate">The container.</param>
    /// <returns>IHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureDryIoc(
        this ConventionContextBuilder builder,
        Action<IConventionContext, IConfiguration, IServiceCollection, IContainer> @delegate
    )
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.AppendDelegate(
            new DryIocConvention(
                (context, configuration, services, container) =>
                {
                    @delegate(context, configuration, services, container);
                    return container;
                }
            )
        );
        return builder;
    }

    /// <summary>
    /// Uses the DryIoc.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="delegate">The container.</param>
    /// <returns>IHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureDryIoc(this ConventionContextBuilder builder, Func<IServiceCollection, IContainer, IContainer> @delegate)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.AppendDelegate(new DryIocConvention((context, configuration, services, container) => @delegate(services, container)));
        return builder;
    }

    /// <summary>
    /// Uses the DryIoc.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="delegate">The container.</param>
    /// <returns>IHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureDryIoc(this ConventionContextBuilder builder, Action<IConfiguration, IServiceCollection, IContainer> @delegate)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.AppendDelegate(
            new DryIocConvention(
                (context, configuration, services, container) =>
                {
                    @delegate(configuration, services, container);
                    return container;
                }
            )
        );
        return builder;
    }

    /// <summary>
    /// Uses the DryIoc.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="delegate">The container.</param>
    /// <returns>IHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureDryIoc(
        this ConventionContextBuilder builder,
        Func<IConfiguration, IServiceCollection, IContainer, IContainer> @delegate
    )
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.AppendDelegate(new DryIocConvention((context, configuration, services, container) => @delegate(configuration, services, container)));
        return builder;
    }

    /// <summary>
    /// Uses the DryIoc.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="delegate">The container.</param>
    /// <returns>IHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureDryIoc(this ConventionContextBuilder builder, Action<IServiceCollection, IContainer> @delegate)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.AppendDelegate(
            new DryIocConvention(
                (context, configuration, services, container) =>
                {
                    @delegate(services, container);
                    return container;
                }
            )
        );
        return builder;
    }

    /// <summary>
    /// Uses the DryIoc.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="delegate">The container.</param>
    /// <returns>IHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureDryIoc(this ConventionContextBuilder builder, Func<IContainer, IContainer> @delegate)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.AppendDelegate(new DryIocConvention((context, configuration, services, container) => @delegate(container)));
        return builder;
    }

    /// <summary>
    /// Uses the DryIoc.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="delegate">The container.</param>
    /// <returns>IHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureDryIoc(this ConventionContextBuilder builder, Action<IContainer> @delegate)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.AppendDelegate(
            new DryIocConvention(
                (context, configuration, services, container) =>
                {
                    @delegate(container);
                    return container;
                }
            )
        );
        return builder;
    }
}
