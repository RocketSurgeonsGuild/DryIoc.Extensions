using DryIoc;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.DryIoc;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Extensions.DryIoc.Tests
{
    public class TestHostTests : AutoFakeTest
    {
        [Fact]
        public void IntegratesWithTestHost()
        {
            var host = TestHost.For(typeof(TestHostTests)).Create()
               .Configure(c => c.ConfigureRocketSurgery(x => x.Set(new DryIocOptions() { NoMoreRegistrationAllowed = false }).UseDryIoc().AppendConvention<TestConvention>()));
            var services = host.Build().Services.GetRequiredService<IContainer>();

            // We are using dryioc...
            Populate(services);

            Container.IsRegistered<AService>().Should().BeTrue();
        }

        public TestHostTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

        class TestConvention : IServiceConvention, IDryIocConvention
        {
            public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services) { }

            public IContainer Register(IConventionContext conventionContext, IConfiguration configuration, IServiceCollection services, IContainer container)
            {
                container.UseInstance(new AService());
                return container;
            }
        }

        class AService
        {
            public string Value { get; } = "A Value";
        }
    }
}