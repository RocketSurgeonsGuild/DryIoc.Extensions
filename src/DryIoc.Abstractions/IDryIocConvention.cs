using JetBrains.Annotations;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Extensions.DryIoc
{
    /// <summary>
    /// IDryIocConvention
    /// Implements the <see cref="IConvention{TContext}" />
    /// </summary>
    /// <seealso cref="IConvention{IDryIocConventionContext}" />
    [PublicAPI]
    public interface IDryIocConvention : IConvention<IDryIocConventionContext> { }
}