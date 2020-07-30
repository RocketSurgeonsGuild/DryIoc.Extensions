using System;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DryIoc;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Hosting;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting
{
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
        /// <param name="@delegate">The container.</param>
        /// <returns>IHostBuilder.</returns>
        public static IConventionHostBuilder ConfigureDryIoc([NotNull] this IConventionHostBuilder builder, DryIocConventionDelegate @delegate)
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
        /// <param name="container">The container.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder UseDryIoc([NotNull] this IConventionHostBuilder builder, IContainer? container = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.ConfigureHosting(
                hosting => hosting.Builder.UseServiceProviderFactory(
                    context =>
                        new ServicesBuilderServiceProviderFactory(
                            collection =>
                                new DryIocBuilder(
                                    context.HostingEnvironment,
                                    context.Configuration,
                                    builder.Get<IConventionScanner>(),
                                    builder.Get<IAssemblyProvider>(),
                                    builder.Get<IAssemblyCandidateFinder>(),
                                    collection,
                                    container ?? new Container().WithDependencyInjectionAdapter(),
                                    builder.Get<ILogger>(),
                                    builder.ServiceProperties
                                )
                        )
                )
            );
        }
    }
}