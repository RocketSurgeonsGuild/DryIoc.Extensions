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
        /// <param name="container">The container.</param>
        /// <returns>IHostBuilder.</returns>
        public static IHostBuilder UseDryIoc([NotNull] this IHostBuilder builder, IContainer? container = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.GetConventions().UseDryIoc(container);
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

            builder.ConfigureHosting(hosting => hosting.Builder.ConfigureServices(
                (context, services) =>
                {
                    hosting.Builder.UseServiceProviderFactory(
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
                    );
                }
            ));
            return builder;
        }
    }
}