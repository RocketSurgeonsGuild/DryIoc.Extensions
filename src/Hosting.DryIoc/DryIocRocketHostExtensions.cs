using System;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.DryIoc;
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
        /// <returns>IRocketHostBuilder.</returns>
        public static IRocketHostBuilder UseDryIoc([NotNull] this IRocketHostBuilder builder, IContainer? container = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Builder.ConfigureServices(
                (context, services) =>
                {
                    builder.Builder.UseServiceProviderFactory(
                        new ServicesBuilderServiceProviderFactory(
                            collection =>
                                new DryIocBuilder(
                                    context.HostingEnvironment.Convert(),
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
            );
            return builder;
        }
    }
}