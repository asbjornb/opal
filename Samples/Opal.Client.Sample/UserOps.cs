namespace Opal.Client.Sample;

public class UserOps
{
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA1822 // Mark members as static
    [Callable("Triggers a manual user reimport", false)]
    public void ReimportUser(int userId)
    {
        Console.WriteLine($"Reimporting user with ID: {userId}");
    }

    [Callable("Get user information", true)]
    public object GetUserDetails(int userId)
    {
        Console.WriteLine($"Getting details for user with ID: {userId}");
        return new { Id = userId, Name = "Sample User", Email = "user@example.com" };
    }

    public void NonCallableMethod()
    {
        Console.WriteLine("This method should not be discovered");
    }
#pragma warning restore CA1822 // Mark members as static
#pragma warning restore IDE0079 // Remove unnecessary suppression
}