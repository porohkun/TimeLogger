using System;

namespace TimeLogger.Attributes
{
    public class AsSingletoneAttribute : ContainerRegisterAttribute
    {
        /// <inheritdoc/>
        public AsSingletoneAttribute() : base() { }

        /// <inheritdoc/>
        public AsSingletoneAttribute(params Type[] abstractions) : base(abstractions) { }
    }
}
