using System.Collections.Generic;
using System.Reflection;
using Rocket.Surgery.Conventions.DryIoc;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Extensions.DryIoc.Tests
{
    internal class TestAssemblyProvider : IAssemblyProvider
    {
        public IEnumerable<Assembly> GetAssemblies() => new[]
        {
            typeof(DryIocBuilder).GetTypeInfo().Assembly,
            typeof(TestAssemblyProvider).GetTypeInfo().Assembly
        };
    }
}