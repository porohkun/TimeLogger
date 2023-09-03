// ReSharper disable once CheckNamespace
namespace System
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> GetInheritors(this Type baseType,
            bool includeAbstracts = false,
            bool includeSelf = false,
            bool includeInterfaces = false)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                .Where(t =>
                {
                    if (!baseType.IsAssignableFrom(t)) return false;
                    if (!includeSelf && t == baseType) return false;
                    if (!includeAbstracts && t.IsAbstract) return false;
                    if (!includeInterfaces && t.IsInterface) return false;
                    return true;
                });
        }
    }
}
