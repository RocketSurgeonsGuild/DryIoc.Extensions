using System;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DryIoc;

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
        /// <param name="delegate">The container.</param>
        /// <returns>IHostBuilder.</returns>
        public static ConventionContextBuilder ConfigureDryIoc([NotNull] this ConventionContextBuilder builder, DryIocConvention @delegate)
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
        public static ConventionContextBuilder UseDryIoc([NotNull] this ConventionContextBuilder builder, IContainer? container = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.ConfigureHosting((context, builder) => builder.UseServiceProviderFactory(new DryIocConventionServiceProviderFactory(context, container)));
        }
    }
}