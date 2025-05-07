namespace Opal.Client.Sample;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Opal.Client Sample Application");
        Console.WriteLine("-----------------------------");

        // Scan the UserOps class for callable methods
        var methods = MethodScanner.ScanType(typeof(UserOps)).ToList();

        Console.WriteLine($"Found {methods.Count} callable methods:");

        foreach (var method in methods)
        {
            // Display basic method information
            Console.WriteLine($"- {method.MethodName}({FormatParameters(method)}) [ReadOnly={method.ReadOnly}]: {method.Description}");
        }
    }

    private static string FormatParameters(DiscoveredMethod method)
    {
        return string.Join(", ", method.Parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
    }
}