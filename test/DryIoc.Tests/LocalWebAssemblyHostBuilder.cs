using System;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Extensions.DryIoc.Tests;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Reflection;
using System.Linq;

#pragma warning disable CA1040, CA1034, CA2000, IDE0058, RCS1021

[assembly: Convention(typeof(DryIocBuilderTests.AbcConvention))]
[assembly: Convention(typeof(DryIocBuilderTests.OtherConvention))]

namespace Rocket.Surgery.Extensions.DryIoc.Tests
{
    /// <summary>
    /// A builder for configuring and creating a <see cref="WebAssemblyHost"/>.
    /// </summary>
    class LocalWebAssemblyHostBuilder : IWebAssemblyHostBuilder
    {
        private Func<IServiceProvider> _createServiceProvider;

        /// <summary>
        /// Creates an instance of <see cref="WebAssemblyHostBuilder"/> using the most common
        /// conventions and settings.
        /// </summary>
        /// <param name="args">The argument passed to the application's main method.</param>
        /// <returns>A <see cref="WebAssemblyHostBuilder"/>.</returns>
        public static LocalWebAssemblyHostBuilder CreateDefault(string[] args = default)
        {
            // We don't use the args for anything right now, but we want to accept them
            // here so that it shows up this way in the project templates.
            args ??= Array.Empty<string>();
            var builder = new LocalWebAssemblyHostBuilder();

            // Right now we don't have conventions or behaviors that are specific to this method
            // however, making this the default for the template allows us to add things like that
            // in the future, while giving `new LocalWebAssemblyHostBuilder` as an opt-out of opinionated
            // settings.
            return builder;
        }

        /// <summary>
        /// Creates an instance of <see cref="LocalWebAssemblyHostBuilder"/> with the minimal configuration.
        /// </summary>
        internal LocalWebAssemblyHostBuilder()
        {
            // Private right now because we don't have much reason to expose it. This can be exposed
            // in the future if we want to give people a choice between CreateDefault and something
            // less opinionated.
            Configuration = new WebAssemblyHostConfiguration();
            RootComponents = new RootComponentMappingCollection();
            Services = new ServiceCollection();
            Logging = new LoggingBuilder(Services);

            var hostEnvironment = InitializeEnvironment();
            HostEnvironment = hostEnvironment;

            _createServiceProvider = () => Services.BuildServiceProvider(validateScopes: hostEnvironment.IsDevelopment());
        }

        private IWebAssemblyHostEnvironment InitializeEnvironment()
        {
            var hostEnvironment = new WebAssemblyHostEnvironment("applicationEnvironment", "http://localhost/");

            Services.AddSingleton<IWebAssemblyHostEnvironment>(hostEnvironment);

            return hostEnvironment;
        }

        /// <summary>
        /// Gets an <see cref="WebAssemblyHostConfiguration"/> that can be used to customize the application's
        /// configuration sources and read configuration attributes.
        /// </summary>
        public WebAssemblyHostConfiguration Configuration { get; }

        /// <summary>
        /// Gets the collection of root component mappings configured for the application.
        /// </summary>
        public RootComponentMappingCollection RootComponents { get; }

        /// <summary>
        /// Gets the service collection.
        /// </summary>
        public IServiceCollection Services { get; }

        /// <summary>
        /// Gets information about the app's host environment.
        /// </summary>
        public IWebAssemblyHostEnvironment HostEnvironment { get; }

        /// <summary>
        /// Gets the logging builder for configuring logging services.
        /// </summary>
        public Microsoft.Extensions.Logging.ILoggingBuilder Logging { get; }

        /// <summary>
        /// Registers a <see cref="IServiceProviderFactory{TBuilder}" /> instance to be used to create the <see cref="IServiceProvider" />.
        /// </summary>
        /// <param name="factory">The <see cref="IServiceProviderFactory{TBuilder}" />.</param>
        /// <param name="configure">
        /// A delegate used to configure the <typeparamref T="TBuilder" />. This can be used to configure services using
        /// APIS specific to the <see cref="IServiceProviderFactory{TBuilder}" /> implementation.
        /// </param>
        /// <typeparam name="TBuilder">The type of builder provided by the <see cref="IServiceProviderFactory{TBuilder}" />.</typeparam>
        /// <remarks>
        /// <para>
        /// <see cref="ConfigureContainer{TBuilder}(IServiceProviderFactory{TBuilder}, Action{TBuilder})"/> is called by <see cref="Build"/>
        /// and so the delegate provided by <paramref name="configure"/> will run after all other services have been registered.
        /// </para>
        /// <para>
        /// Multiple calls to <see cref="ConfigureContainer{TBuilder}(IServiceProviderFactory{TBuilder}, Action{TBuilder})"/> will replace
        /// the previously stored <paramref name="factory"/> and <paramref name="configure"/> delegate.
        /// </para>
        /// </remarks>
        public void ConfigureContainer<TBuilder>(IServiceProviderFactory<TBuilder> factory, Action<TBuilder> configure = null)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            _createServiceProvider = () =>
            {
                var container = factory.CreateBuilder(Services);
                configure?.Invoke(container);
                return factory.CreateServiceProvider(container);
            };
        }

        /// <summary>
        /// Builds a <see cref="WebAssemblyHost"/> instance based on the configuration of this builder.
        /// </summary>
        /// <returns>A <see cref="WebAssemblyHost"/> object.</returns>
        public WebAssemblyHost Build()
        {
            // Intentionally overwrite configuration with the one we're creating.
            Services.AddSingleton<IConfiguration>(Configuration);

            // A Blazor application always runs in a scope. Since we want to make it possible for the user
            // to configure services inside *that scope* inside their startup code, we create *both* the
            // service provider and the scope here.
            var services = _createServiceProvider();
            var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope();

            return Activator.CreateInstance(
                typeof(WebAssemblyHost),
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance,
                null,
                new object[] { services, scope, Configuration, RootComponents.ToArray() },
                null
            ) as WebAssemblyHost;
        }
    }
}