using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using DryIoc;
using FakeItEasy;
using FakeItEasy.Creation;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.DryIoc.Tests;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;
using Rocket.Surgery.Conventions.CommandLine;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.DryIoc;
using Rocket.Surgery.WebAssembly.Hosting;

namespace Rocket.Surgery.Extensions.DryIoc.Tests
{
    public class WebAssemblyDryIocBuilderTests : AutoFakeTest
    {
        [Fact]
        public async Task Should_Integrate_With_DryIoc()
        {
            var builder = LocalWebAssemblyHostBuilder.CreateDefault()
               .ConfigureRocketSurgery(rb => rb.UseAssemblies(new[] { typeof(WebAssemblyDryIocBuilderTests).Assembly }).UseDryIoc());

            var was = builder.Build();
            was.Services.GetRequiredService<IContainer>().Should().NotBeNull();
        }

        public WebAssemblyDryIocBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            AutoFake.Provide<IDictionary<object, object?>>(new ServiceProviderDictionary());
        }
    }
}