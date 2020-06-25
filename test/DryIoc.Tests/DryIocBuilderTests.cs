using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using DryIoc;
using FakeItEasy;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.DryIoc.Tests;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting;
using Xunit;
using Xunit.Abstractions;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions.CommandLine;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.DryIoc;

#pragma warning disable CA1040, CA1034, CA2000, IDE0058, RCS1021

[assembly: Convention(typeof(DryIocBuilderTests.AbcConvention))]
[assembly: Convention(typeof(DryIocBuilderTests.OtherConvention))]

namespace Rocket.Surgery.Extensions.DryIoc.Tests
{
    public class DryIocBuilderTests : AutoFakeTest
    {
        [Fact]
        public void Constructs()
        {
            var services = AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            AutoFake.Provide<IContainer>(new Container());
            var assemblyProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IConventionScanner, BasicConventionScanner>();
            var servicesBuilder = AutoFake.Resolve<DryIocBuilder>();

            servicesBuilder.AssemblyProvider.Should().BeSameAs(assemblyProvider);
            servicesBuilder.AssemblyCandidateFinder.Should().NotBeNull();
            servicesBuilder.Services.Should().BeSameAs(services);
            servicesBuilder.Configuration.Should().NotBeNull();
            servicesBuilder.Environment.Should().NotBeNull();

            Action a = () => { servicesBuilder.PrependConvention(A.Fake<IDryIocConvention>()); };
            a.Should().NotThrow();
            a = () => { servicesBuilder.PrependDelegate(delegate { }); };
            a.Should().NotThrow();
            a = () => { servicesBuilder.ConfigureContainer(_ => { }); };
            a.Should().NotThrow();
            a = () => { servicesBuilder.ConfigureContainer(_ => _); };
            a.Should().NotThrow();
        }

        [Fact]
        public void StoresAndReturnsItems()
        {
            AutoFake.Provide<IDictionary<object, object>>(new Dictionary<object, object>());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            AutoFake.Provide<IContainer>(new Container());
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IConventionScanner, BasicConventionScanner>();
            var servicesBuilder = AutoFake.Resolve<DryIocBuilder>();

            var value = new object();
            servicesBuilder[string.Empty] = value;
            servicesBuilder[string.Empty].Should().BeSameAs(value);
        }

        [Fact]
        public void IgnoreNonExistentItems()
        {
            AutoFake.Provide<IDictionary<object, object>>(new Dictionary<object, object>());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            AutoFake.Provide<IContainer>(new Container());
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IConventionScanner, BasicConventionScanner>();
            var servicesBuilder = AutoFake.Resolve<DryIocBuilder>();

            servicesBuilder[string.Empty].Should().BeNull();
        }

        [Fact]
        public void AddConventions()
        {
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            AutoFake.Provide<IContainer>(new Container());
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            var servicesBuilder = AutoFake.Resolve<DryIocBuilder>();

            var convention = A.Fake<IDryIocConvention>();

            servicesBuilder.PrependConvention(convention);

            A.CallTo(() => AutoFake.Resolve<IConventionScanner>().PrependConvention(A<IEnumerable<IConvention>>._))
               .MustHaveHappened();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithCore()
        {
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            AutoFake.Provide<IContainer>(new Container());
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IConventionScanner, BasicConventionScanner>();
            var servicesBuilder = AutoFake.Resolve<DryIocBuilder>();

            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc>()));
            servicesBuilder.Services.AddSingleton(A.Fake<IAbc2>());

            var items = servicesBuilder.Build();
            items.Resolve<IAbc>(IfUnresolved.ReturnDefault).Should().NotBeNull();
            items.Resolve<IAbc2>(IfUnresolved.ReturnDefault).Should().NotBeNull();
            items.Resolve<IAbc3>(IfUnresolved.ReturnDefault).Should().BeNull();
            items.Resolve<IAbc4>(IfUnresolved.ReturnDefault).Should().BeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithApplication()
        {
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            AutoFake.Provide<IContainer>(new Container());
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IConventionScanner, BasicConventionScanner>();
            var servicesBuilder = AutoFake.Resolve<DryIocBuilder>();

            servicesBuilder.ConfigureContainer(
                c => c.RegisterInstance(A.Fake<IAbc>())
            );
            servicesBuilder.Services.AddSingleton(A.Fake<IAbc2>());
            servicesBuilder.ConfigureContainer(
                c => c.RegisterInstance(A.Fake<IAbc4>())
            );

            var items = servicesBuilder.Build();
            items.Resolve<IAbc>(IfUnresolved.ReturnDefault).Should().NotBeNull();
            items.Resolve<IAbc2>(IfUnresolved.ReturnDefault).Should().NotBeNull();
            items.Resolve<IAbc3>(IfUnresolved.ReturnDefault).Should().BeNull();
            items.Resolve<IAbc4>(IfUnresolved.ReturnDefault).Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithSystem()
        {
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            AutoFake.Provide<IContainer>(new Container());
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IConventionScanner, BasicConventionScanner>();
            var servicesBuilder = AutoFake.Resolve<DryIocBuilder>();

            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc3>()));
            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc4>()));

            var items = servicesBuilder.Build();
            items.Resolve<IAbc>(IfUnresolved.ReturnDefault).Should().BeNull();
            items.Resolve<IAbc2>(IfUnresolved.ReturnDefault).Should().BeNull();
            items.Resolve<IAbc3>(IfUnresolved.ReturnDefault).Should().NotBeNull();
            items.Resolve<IAbc4>(IfUnresolved.ReturnDefault).Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithCore_ServiceProvider()
        {
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            AutoFake.Provide<IContainer>(new Container());
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IConventionScanner, BasicConventionScanner>();
            var servicesBuilder = AutoFake.Resolve<DryIocBuilder>();

            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc>()));
            servicesBuilder.Services.AddSingleton(A.Fake<IAbc2>());

            var items = servicesBuilder.Build();

            var sp = items.Resolve<IServiceProvider>();
            sp.GetService<IAbc>().Should().NotBeNull();
            sp.GetService<IAbc2>().Should().NotBeNull();
            sp.GetService<IAbc3>().Should().BeNull();
            sp.GetService<IAbc4>().Should().BeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithApplication_ServiceProvider()
        {
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            AutoFake.Provide<IContainer>(new Container());
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IConventionScanner, BasicConventionScanner>();
            var servicesBuilder = AutoFake.Resolve<DryIocBuilder>();

            servicesBuilder.ConfigureContainer(c => c.UseInstance(A.Fake<IAbc>()));
            servicesBuilder.Services.AddSingleton(A.Fake<IAbc2>());
            servicesBuilder.ConfigureContainer(c => c.UseInstance(A.Fake<IAbc4>()));

            var items = servicesBuilder.Build();
            var sp = items.Resolve<IServiceProvider>();
            sp.GetService<IAbc>().Should().NotBeNull();
            sp.GetService<IAbc2>().Should().NotBeNull();
            sp.GetService<IAbc3>().Should().BeNull();
            sp.GetService<IAbc4>().Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithSystem_ServiceProvider()
        {
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            AutoFake.Provide<IContainer>(new Container());
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IConventionScanner, BasicConventionScanner>();
            var servicesBuilder = AutoFake.Resolve<DryIocBuilder>();

            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc3>()));
            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc4>()));

            var items = servicesBuilder.Build();
            var sp = items.Resolve<IServiceProvider>();
            sp.GetService<IAbc>().Should().BeNull();
            sp.GetService<IAbc2>().Should().BeNull();
            sp.GetService<IAbc3>().Should().NotBeNull();
            sp.GetService<IAbc4>().Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithSystem_UsingConvention()
        {
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            AutoFake.Provide<IContainer>(new Container());
            var assemblyProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceProvider>(new ServiceProviderDictionary());
            A.CallTo(() => AutoFake.Provide(A.Fake<IAssemblyCandidateFinder>()).GetCandidateAssemblies(A<IEnumerable<string>>._))
               .Returns(assemblyProvider.GetAssemblies());
            AutoFake.Provide<IConventionScanner, AggregateConventionScanner>();

            var servicesBuilder = AutoFake.Resolve<DryIocBuilder>();

            var items = servicesBuilder.Build();
            items.Resolve<IAbc>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
            items.Resolve<IAbc2>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
            items.Resolve<IAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
            items.Resolve<IAbc4>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithSystem_UsingConvention_IncludingOtherBits()
        {
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            AutoFake.Provide<IContainer>(new Container());
            var assemblyProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceProvider>(new ServiceProviderDictionary());
            A.CallTo(() => AutoFake.Provide(A.Fake<IAssemblyCandidateFinder>()).GetCandidateAssemblies(A<IEnumerable<string>>._))
               .Returns(assemblyProvider.GetAssemblies());
            AutoFake.Provide<IConventionScanner, AggregateConventionScanner>();

            var servicesBuilder = AutoFake.Resolve<DryIocBuilder>();

            var items = servicesBuilder.Build();
            items.Resolve<IAbc>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
            items.Resolve<IAbc2>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
            items.Resolve<IAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
            items.Resolve<IAbc4>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
            items.Resolve<IOtherAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
            items.Resolve<IOtherAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        }

        [Fact]
        public void SendsNotificationThrough_OnBuild_Observable_ForMicrosoftExtensions()
        {
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            AutoFake.Provide<IContainer>(new Container());
            var assemblyProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IConventionScanner, BasicConventionScanner>();
            var servicesBuilder = AutoFake.Resolve<DryIocBuilder>();

            A.CallTo(
                    () => AutoFake.Resolve<IAssemblyCandidateFinder>().GetCandidateAssemblies(A<IEnumerable<string>>._)
                )
               .Returns(assemblyProvider.GetAssemblies());

            var observer = A.Fake<IObserver<IServiceProvider>>();
            ( (IServiceConventionContext)servicesBuilder ).OnBuild.Subscribe(observer);

            var items = servicesBuilder.Build();

            A.CallTo(() => observer.OnNext(A<IServiceProvider>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void SendsNotificationThrough_OnBuild_Observable_ForDryIoc()
        {
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            AutoFake.Provide<IContainer>(new Container());
            var assemblyProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IConventionScanner, BasicConventionScanner>();
            var servicesBuilder = AutoFake.Resolve<DryIocBuilder>(); ;

            A.CallTo(
                    () => AutoFake.Resolve<IAssemblyCandidateFinder>().GetCandidateAssemblies(A<IEnumerable<string>>._)
                )
               .Returns(assemblyProvider.GetAssemblies());

            var observer = A.Fake<IObserver<IServiceProvider>>();
            var observerContainer = A.Fake<IObserver<IContainer>>();
            servicesBuilder.OnContainerBuild.Subscribe(observerContainer);
            servicesBuilder.OnBuild.Subscribe(observer);

            var container = servicesBuilder.Build();

            A.CallTo(() => observer.OnNext(A<IServiceProvider>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => observerContainer.OnNext(A<IContainer>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Should_Integrate_With_DryIoc()
        {
            var builder = Host.CreateDefaultBuilder(Array.Empty<string>())
               .ConfigureRocketSurgery(
                    rb => rb
                       .UseScannerUnsafe(new BasicConventionScanner(A.Fake<IServiceProviderDictionary>()))
                       .UseDryIoc()
                       .UseAssemblyCandidateFinder(
                            new DefaultAssemblyCandidateFinder(new[] { typeof(DryIocBuilderTests).Assembly })
                        )
                       .UseAssemblyProvider(new DefaultAssemblyProvider(new[] { typeof(DryIocBuilderTests).Assembly }))
                       .AppendDelegate(
                            new CommandLineConventionDelegate(c => c.OnRun(state => 1337)),
                            new CommandLineConventionDelegate(c => c.OnRun(state => 1337))
                        )
                );

            using var host = builder.Build();
            await host.StartAsync().ConfigureAwait(false);
            var container = host.Services.GetRequiredService<IContainer>();
            container.Should().NotBeNull();
            await host.StopAsync().ConfigureAwait(false);
        }

        public DryIocBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper)
            => AutoFake.Provide<DiagnosticSource>(new DiagnosticListener("Test"));

        public interface IAbc { }

        public interface IAbc2 { }

        public interface IAbc3 { }

        public interface IAbc4 { }

        public interface IOtherAbc3 { }

        public interface IOtherAbc4 { }

        public class AbcConvention : IDryIocConvention
        {
            public void Register([NotNull] IDryIocConventionContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                context.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc>()));
                context.Services.AddSingleton(A.Fake<IAbc2>());
                context.ConfigureContainer(c => { });
            }
        }

        public class OtherConvention : IServiceConvention
        {
            public void Register([NotNull] IServiceConventionContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                context.Services.AddSingleton(A.Fake<IOtherAbc3>());
            }
        }
    }
}