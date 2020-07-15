using System;
using System.Collections.Generic;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.DryIoc.Internals;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions.DryIoc
{
    /// <summary>
    /// DryIocBuilder.
    /// Implements the <see cref="ConventionBuilder{TBuilder,TConvention,TDelegate}" />
    /// Implements the <see cref="IDryIocBuilder" />
    /// Implements the <see cref="IServicesBuilder" />
    /// Implements the <see cref="IDryIocConventionContext" />
    /// </summary>
    /// <seealso cref="ConventionBuilder{IDryIocBuilder, IDryIocConvention, DryIocConventionDelegate}" />
    /// <seealso cref="IDryIocBuilder" />
    /// <seealso cref="IServicesBuilder" />
    /// <seealso cref="IDryIocConventionContext" />
    public class DryIocBuilder : ConventionBuilder<IDryIocBuilder, IDryIocConvention, DryIocConventionDelegate>,
                                 IDryIocBuilder,
                                 IServicesBuilder,
                                 IDryIocConventionContext
    {
        private readonly GenericObservableObservable<IContainer> _containerObservable;
        private readonly GenericObservableObservable<IServiceProvider> _serviceProviderOnBuild;
        private IContainer _container => this.Get<IContainer>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DryIocBuilder" /> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="scanner">The scanner.</param>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <param name="services">The services.</param>
        /// <param name="container">The container.</param>
        /// <param name="diagnosticSource">The diagnostic source</param>
        /// <param name="properties">The properties</param>
        /// <exception cref="ArgumentNullException">
        /// environment
        /// or
        /// container
        /// or
        /// configuration
        /// or
        /// services
        /// </exception>
        public DryIocBuilder(
            IHostEnvironment environment,
            IConfiguration configuration,
            IConventionScanner scanner,
            IAssemblyProvider assemblyProvider,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IServiceCollection services,
            IContainer container,
            ILogger diagnosticSource,
            IDictionary<object, object?> properties
        )
            : base(scanner, assemblyProvider, assemblyCandidateFinder, properties)
        {
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Services = services ?? throw new ArgumentNullException(nameof(services));
            Logger = diagnosticSource ?? throw new ArgumentNullException(nameof(diagnosticSource));

            _containerObservable = new GenericObservableObservable<IContainer>(Logger);
            _serviceProviderOnBuild = new GenericObservableObservable<IServiceProvider>(Logger);
            this.Set(container ?? throw new ArgumentNullException(nameof(container)));
        }

        /// <summary>
        /// Configures the container.
        /// </summary>
        /// <param name="configureContainer">The configuration delegate.</param>
        /// <returns>IDryIocConventionContext.</returns>
        public DryIocBuilder ConfigureContainer(Func<IContainer, IContainer> configureContainer)
        {
            this.Set(configureContainer?.Invoke(_container) ?? _container);
            return this;
        }

        /// <summary>
        /// Configures the container.
        /// </summary>
        /// <param name="configureContainer">The configuration delegate.</param>
        /// <returns>IDryIocConventionContext.</returns>
        public DryIocBuilder ConfigureContainer(Action<IContainer> configureContainer)
        {
            configureContainer?.Invoke(_container);
            return this;
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns>IContainer.</returns>
        public IContainer Build()
        {
            var options = this.GetOrAdd(() => new DryIocOptions());
            Composer.Register(
                Scanner,
                this,
                typeof(IServiceConvention),
                typeof(IDryIocConvention),
                typeof(ServiceConventionDelegate),
                typeof(DryIocConventionDelegate)
            );

            _container.Populate(Services);

            _serviceProviderOnBuild.Send(_container);
            _containerObservable.Send(_container);

            this.Set(options.NoMoreRegistrationAllowed ? _container.WithNoMoreRegistrationAllowed() : _container);

            return _container;
        }

        /// <summary>
        /// Gets the on container build.
        /// </summary>
        /// <value>The on container build.</value>
        /// <inheritdoc />
        public IObservable<IContainer> OnContainerBuild => _containerObservable;

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <value>The services.</value>
        public IServiceCollection Services { get; }

        /// <summary>
        /// The environment that this convention is running
        /// Based on IHostEnvironment / IHostingEnvironment
        /// </summary>
        /// <value>The environment.</value>
        public IHostEnvironment Environment { get; }

        /// <summary>
        /// Gets the on build.
        /// </summary>
        /// <value>The on build.</value>
        /// <inheritdoc />
        public IObservable<IServiceProvider> OnBuild => _serviceProviderOnBuild;

        /// <summary>
        /// A logger that is configured to work with each convention item
        /// </summary>
        /// <value>The logger.</value>
        /// <inheritdoc />
        public ILogger Logger { get; }

        IDryIocConventionContext IDryIocConventionContext.ConfigureContainer(Func<IContainer, IContainer> configureContainer)
            => ConfigureContainer(configureContainer);

        IDryIocConventionContext IDryIocConventionContext.ConfigureContainer(Action<IContainer> configureContainer) => ConfigureContainer(configureContainer);

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.
            AppendConvention(params IServiceConvention[] conventions)
        {
            Scanner.AppendConvention(conventions);
            return this;
        }

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.
            AppendConvention(IEnumerable<IServiceConvention> conventions)
        {
            Scanner.AppendConvention(conventions);
            return this;
        }

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.
            AppendConvention<T>()
        {
            Scanner.AppendConvention<T>();
            return this;
        }

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.
            PrependConvention(params IServiceConvention[] conventions)
        {
            Scanner.PrependConvention(conventions);
            return this;
        }

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.
            PrependConvention(IEnumerable<IServiceConvention> conventions)
        {
            Scanner.PrependConvention(conventions);
            return this;
        }

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.
            PrependConvention<T>()
        {
            Scanner.PrependConvention<T>();
            return this;
        }

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.
            PrependDelegate(params ServiceConventionDelegate[] delegates)
        {
            Scanner.PrependDelegate(delegates);
            return this;
        }

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.
            PrependDelegate(IEnumerable<ServiceConventionDelegate> delegates)
        {
            Scanner.PrependDelegate(delegates);
            return this;
        }

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.
            AppendDelegate(params ServiceConventionDelegate[] delegates)
        {
            Scanner.AppendDelegate(delegates);
            return this;
        }

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.
            AppendDelegate(IEnumerable<ServiceConventionDelegate> delegates)
        {
            Scanner.AppendDelegate(delegates);
            return this;
        }

        IServiceProvider IServicesBuilder.Build() => Build();
    }
}