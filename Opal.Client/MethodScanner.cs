using System.Reflection;

namespace Opal.Client;

/// <summary>
/// Utility class for discovering methods marked with the <see cref="CallableAttribute"/>.
/// </summary>
public static class MethodScanner
{
    /// <summary>
    /// Scans the specified type for methods marked with the <see cref="CallableAttribute"/>.
    /// </summary>
    /// <param name="type">The type to scan.</param>
    /// <returns>A collection of discovered methods.</returns>
    public static IEnumerable<DiscoveredMethod> ScanType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        // Get all public methods (both instance and static)
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .Where(m => m.GetCustomAttribute<CallableAttribute>() != null);

        var discoveredMethods = new List<DiscoveredMethod>();
        var methodNames = new HashSet<string>();

        foreach (var method in methods)
        {
            // Check for duplicate method names (overloads)
            if (methodNames.Contains(method.Name))
            {
                throw new InvalidOperationException($"Method overloading is not supported: {type.Name}.{method.Name}");
            }

            methodNames.Add(method.Name);

            var attribute = method.GetCustomAttribute<CallableAttribute>();
            discoveredMethods.Add(new DiscoveredMethod(method, attribute!));
        }

        return discoveredMethods;
    }

    /// <summary>
    /// Scans the specified types for methods marked with the <see cref="CallableAttribute"/>.
    /// </summary>
    /// <param name="types">The types to scan.</param>
    /// <returns>A collection of discovered methods.</returns>
    public static IEnumerable<DiscoveredMethod> ScanTypes(IEnumerable<Type> types)
    {
        ArgumentNullException.ThrowIfNull(types);

        var result = new List<DiscoveredMethod>();

        foreach (var type in types)
        {
            result.AddRange(ScanType(type));
        }

        return result;
    }

    /// <summary>
    /// Scans the specified assembly for methods marked with the <see cref="CallableAttribute"/>.
    /// </summary>
    /// <param name="assembly">The assembly to scan.</param>
    /// <returns>A collection of discovered methods.</returns>
    public static IEnumerable<DiscoveredMethod> ScanAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var types = assembly.GetExportedTypes();
        return ScanTypes(types);
    }

    /// <summary>
    /// Scans the specified assemblies for methods marked with the <see cref="CallableAttribute"/>.
    /// </summary>
    /// <param name="assemblies">The assemblies to scan.</param>
    /// <returns>A collection of discovered methods.</returns>
    public static IEnumerable<DiscoveredMethod> ScanAssemblies(IEnumerable<Assembly> assemblies)
    {
        ArgumentNullException.ThrowIfNull(assemblies);

        var result = new List<DiscoveredMethod>();

        foreach (var assembly in assemblies)
        {
            result.AddRange(ScanAssembly(assembly));
        }

        return result;
    }
}