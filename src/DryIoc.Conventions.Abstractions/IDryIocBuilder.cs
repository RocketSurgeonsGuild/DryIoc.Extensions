using System;
using DryIoc;
using JetBrains.Annotations;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Extensions.DryIoc
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