using DryIoc;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Extensions.DryIoc.Tests;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting;
using Xunit;
using Xunit.Abstractions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.DryIoc;

#pragma warning disable CA1040, CA1034, CA2000, IDE0058, RCS1021

[assembly: Convention(typeof(DryIocBuilderTests.AbcConvention))]
[assembly: Convention(typeof(DryIocBuilderTests.OtherConvention))]

namespace Rocket.Surgery.Extensions.DryIoc.Tests;

public class DryIocBuilderTests : AutoFakeTest
{
    [Fact]
    public void ConstructTheContainerAndRegisterWithCore()
    {
        var builder = Host.CreateDefaultBuilder()
                          .ConfigureRocketSurgery(
                               rb => rb
                                    .UseDryIoc()
                                    .DisableConventionAttributes()
                                    .ConfigureDryIoc(
                                         (conventionContext, configuration, services, container) =>
                                         {
                                             container.RegisterInstance(A.Fake<IAbc>());
                                             services.AddSingleton(A.Fake<IAbc2>());
                                             return container;
                                         })
                           );

        var items = builder.Build().Services.GetRequiredService<IResolverContext>();
        items.Resolve<IAbc>(IfUnresolved.ReturnDefault).Should().NotBeNull();
        items.Resolve<IAbc2>(IfUnresolved.ReturnDefault).Should().NotBeNull();
        items.Resolve<IAbc3>(IfUnresolved.ReturnDefault).Should().BeNull();
        items.Resolve<IAbc4>(IfUnresolved.ReturnDefault).Should().BeNull();
    }

    [Fact]
    public void ConstructTheContainerAndRegisterWithApplication()
    {
        var builder = Host.CreateDefaultBuilder()
                          .ConfigureRocketSurgery(
                               rb => rb
                                    .UseDryIoc()
                                    .DisableConventionAttributes()
                                    .ConfigureDryIoc(
                                         (conventionContext, configuration, services, container) =>
                                         {
                                             container.RegisterInstance(A.Fake<IAbc>());
                                             services.AddSingleton(A.Fake<IAbc2>());
                                             container.RegisterInstance(A.Fake<IAbc4>());
                                             return container;
                                         })
                           );

        var items = builder.Build().Services.GetRequiredService<IResolverContext>();
        items.Resolve<IAbc>(IfUnresolved.ReturnDefault).Should().NotBeNull();
        items.Resolve<IAbc2>(IfUnresolved.ReturnDefault).Should().NotBeNull();
        items.Resolve<IAbc3>(IfUnresolved.ReturnDefault).Should().BeNull();
        items.Resolve<IAbc4>(IfUnresolved.ReturnDefault).Should().NotBeNull();
    }

    [Fact]
    public void ConstructTheContainerAndRegisterWithSystem()
    {
        var builder = Host.CreateDefaultBuilder()
                          .ConfigureRocketSurgery(
                               rb => rb
                                    .UseDryIoc()
                                    .DisableConventionAttributes()
                                    .ConfigureDryIoc(
                                         (conventionContext, configuration, services, container) =>
                                         {
                                             container.RegisterInstance(A.Fake<IAbc3>());
                                             container.RegisterInstance(A.Fake<IAbc4>());
                                             return container;
                                         })
                           );

        var items = builder.Build().Services.GetRequiredService<IResolverContext>();
        items.Resolve<IAbc>(IfUnresolved.ReturnDefault).Should().BeNull();
        items.Resolve<IAbc2>(IfUnresolved.ReturnDefault).Should().BeNull();
        items.Resolve<IAbc3>(IfUnresolved.ReturnDefault).Should().NotBeNull();
        items.Resolve<IAbc4>(IfUnresolved.ReturnDefault).Should().NotBeNull();
    }

    [Fact]
    public void ConstructTheContainerAndRegisterWithCore_ServiceProvider()
    {
        var builder = Host.CreateDefaultBuilder()
                          .ConfigureRocketSurgery(
                               rb => rb
                                    .UseDryIoc()
                                    .DisableConventionAttributes()
                                    .ConfigureDryIoc(
                                         (conventionContext, configuration, services, container) =>
                                         {
                                             container.RegisterInstance(A.Fake<IAbc>());
                                             services.AddSingleton(A.Fake<IAbc2>());
                                             return container;
                                         })
                           );

        var items = builder.Build().Services.GetRequiredService<IResolverContext>();

        var sp = items.Resolve<IServiceProvider>();
        sp.GetService<IAbc>().Should().NotBeNull();
        sp.GetService<IAbc2>().Should().NotBeNull();
        sp.GetService<IAbc3>().Should().BeNull();
        sp.GetService<IAbc4>().Should().BeNull();
    }

    [Fact]
    public void ConstructTheContainerAndRegisterWithApplication_ServiceProvider()
    {
        var builder = Host.CreateDefaultBuilder()
                          .ConfigureRocketSurgery(
                               rb => rb
                                    .UseDryIoc()
                                    .DisableConventionAttributes()
                                    .ConfigureDryIoc(
                                         (conventionContext, configuration, services, container) =>
                                         {
                                             container.UseInstance(A.Fake<IAbc>());
                                             services.AddSingleton(A.Fake<IAbc2>());
                                             container.UseInstance(A.Fake<IAbc4>());
                                             return container;
                                         })
                           );

        var items = builder.Build().Services.GetRequiredService<IResolverContext>();
        var sp = items.Resolve<IServiceProvider>();
        sp.GetService<IAbc>().Should().NotBeNull();
        sp.GetService<IAbc2>().Should().NotBeNull();
        sp.GetService<IAbc3>().Should().BeNull();
        sp.GetService<IAbc4>().Should().NotBeNull();
    }

    [Fact]
    public void ConstructTheContainerAndRegisterWithSystem_ServiceProvider()
    {
        var builder = Host.CreateDefaultBuilder()
                          .ConfigureRocketSurgery(
                               rb => rb
                                    .UseDryIoc()
                                    .DisableConventionAttributes()
                                    .ConfigureDryIoc(
                                         (conventionContext, configuration, services, container) =>
                                         {
                                             container.RegisterInstance(A.Fake<IAbc3>());
                                             container.RegisterInstance(A.Fake<IAbc4>());
                                             return container;
                                         })
                           );

        var items = builder.Build().Services.GetRequiredService<IResolverContext>();
        var sp = items.Resolve<IServiceProvider>();
        sp.GetService<IAbc>().Should().BeNull();
        sp.GetService<IAbc2>().Should().BeNull();
        sp.GetService<IAbc3>().Should().NotBeNull();
        sp.GetService<IAbc4>().Should().NotBeNull();
    }

    [Fact]
    public void ConstructTheContainerAndRegisterWithSystem_UsingConvention()
    {
        var builder = Host.CreateDefaultBuilder()
                          .ConfigureRocketSurgery(
                               rb => rb
                                    .UseDryIoc()
                                    .ConfigureDryIoc(
                                         (conventionContext, configuration, services, container) =>
                                         {
                                             return container;
                                         })
                           );

        var items = builder.Build().Services.GetRequiredService<IResolverContext>();
        items.Resolve<IAbc>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<IAbc2>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<IAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
        items.Resolve<IAbc4>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
    }

    [Fact]
    public void ConstructTheContainerAndRegisterWithSystem_UsingConvention_IncludingOtherBits()
    {
        var builder = Host.CreateDefaultBuilder()
                          .ConfigureRocketSurgery(
                               rb => rb
                                    .UseDryIoc()
                                    .ConfigureDryIoc(
                                         (conventionContext, configuration, services, container) =>
                                         {
                                             return container;
                                         })
                           );

        var items = builder.Build().Services.GetRequiredService<IResolverContext>();
        items.Resolve<IAbc>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<IAbc2>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<IAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
        items.Resolve<IAbc4>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
        items.Resolve<IOtherAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<IOtherAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
    }

    [Fact]
    public async Task Should_Integrate_With_DryIoc()
    {
        var builder = Host.CreateDefaultBuilder(Array.Empty<string>())
                          .ConfigureRocketSurgery(rb => rb.UseDryIoc());

        using var host = builder.Build();
        await host.StartAsync().ConfigureAwait(false);
        var container = host.Services.GetRequiredService<IContainer>();
        container.Should().NotBeNull();
        await host.StopAsync().ConfigureAwait(false);
    }

    public DryIocBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
        AutoFake.Provide<IDictionary<object, object?>>(new ServiceProviderDictionary());
    }

    public interface IAbc { }

    public interface IAbc2 { }

    public interface IAbc3 { }

    public interface IAbc4 { }

    public interface IOtherAbc3 { }

    public interface IOtherAbc4 { }

    public class AbcConvention : IDryIocConvention
    {
        public IContainer Register(IConventionContext conventionContext, IConfiguration configuration, IServiceCollection services, IContainer container)
        {
            container.RegisterInstance(A.Fake<IAbc>());
            services.AddSingleton(A.Fake<IAbc2>());
            return container;
        }
    }

    public class OtherConvention : IServiceConvention
    {
        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            services.AddSingleton(A.Fake<IOtherAbc3>());
        }
    }
}