using System;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DryIoc;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.WebAssembly.Hosting;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Components.WebAssembly.Hosting
{
    /// <summary>
    /// Class DryIocRocketHostExtensions.
    /// </summary>
    [PublicAPI]
    public static class WebAssemblyDryIocRocketHostExtensions
    {
        /// <summary>
        /// Uses the DryIoc.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="delegate">The container.</param>
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

            var wasmBuilder = builder.Get<IWebAssemblyHostBuilder>()!;
            wasmBuilder.ConfigureContainer(
                new WebAssemblyServicesBuilderProviderFactory(
                    wasmBuilder,
                    (b, collection) =>
                        new DryIocBuilder(
                            wasmBuilder.Configuration,
                            b.Get<IConventionScanner>()!,
                            b.Get<IAssemblyProvider>()!,
                            b.Get<IAssemblyCandidateFinder>()!,
                            collection,
                            container ?? new Container().WithDependencyInjectionAdapter(),
                            b.Get<ILogger>()!,
                            b.ServiceProperties
                        )
                )
            );
            return builder;
        }
    }
}