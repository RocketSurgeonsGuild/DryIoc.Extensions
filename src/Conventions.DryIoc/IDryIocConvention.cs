using JetBrains.Annotations;

namespace Rocket.Surgery.Conventions.DryIoc
{
    /// <summary>
    /// IDryIocConvention
    /// Implements the <see cref="IConvention{TContext}" />
    /// </summary>
    /// <seealso cref="IConvention{IDryIocConventionContext}" />
    [PublicAPI]
    public interface IDryIocConvention : IConvention<IDryIocConventionContext> { }
}