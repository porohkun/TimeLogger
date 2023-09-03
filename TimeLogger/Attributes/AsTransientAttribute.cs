using System;

namespace TimeLogger.Attributes
{
    public class AsTransientAttribute : ContainerRegisterAttribute
    {
        /// <inheritdoc/>
        public AsTransientAttribute() : base() { }

        /// <inheritdoc/>
        public AsTransientAttribute(params Type[] abstractions) : base(abstractions) { }
    }
}
