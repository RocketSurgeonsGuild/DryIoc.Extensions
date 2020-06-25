using JetBrains.Annotations;

namespace Rocket.Surgery.Conventions.DryIoc
{
    /// <summary>
    /// IDryIocBuilder.
    /// Implements the <see cref="IConventionBuilder{TBuilder,TConvention,TDelegate}" />
    /// </summary>
    /// <seealso cref="IConventionBuilder{IDryIocBuilder, IDryIocConvention, DryIocConventionDelegate}" />
    [PublicAPI]
    public interface
        IDryIocBuilder : IConventionBuilder<IDryIocBuilder, IDryIocConvention, DryIocConventionDelegate>
    {
    }
}