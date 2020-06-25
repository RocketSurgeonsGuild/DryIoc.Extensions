namespace Rocket.Surgery.Conventions.DryIoc
{
    /// <summary>
    /// Options for building the DryIoc Container
    /// </summary>
    public class DryIocOptions
    {
        /// <summary>
        /// Prevents additional container registrations (defaults to true)
        /// </summary>
        public bool NoMoreRegistrationAllowed { get; set; } = true;
    }
}