public static class Tools
{
    public static string GenerateId()
    {
        return System.Guid.NewGuid().ToString();
    }
}