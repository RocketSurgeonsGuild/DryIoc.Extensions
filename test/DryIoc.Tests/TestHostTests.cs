using DryIoc;
using FluentAssertions;
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
            var host = TestHost.For(typeof(TestHostTests))
               .Create(
                    c => c
                       .ConfigureRocketSurgery(x => ConventionHostBuilderExtensions.Set((IConventionHostBuilder)x, new DryIocOptions() { NoMoreRegistrationAllowed = false}).UseDryIoc().AppendConvention<TestConvention>())
                );
            var services = host.Build().Services.GetRequiredService<IContainer>();

            // We are using dryioc...
            Populate(services);

            Container.IsRegistered<AService>().Should().BeTrue();
        }

        public TestHostTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

        class TestConvention : IServiceConvention, IDryIocConvention
        {
            public TestConvention() { }

            public void Register(IServiceConventionContext context) { }

            public void Register(IDryIocConventionContext context)
            {
                context.ConfigureContainer(c => c.UseInstance(new AService()));
            }
        }

        class AService
        {
            public string Value { get; } = "A Value";
        }
    }
}