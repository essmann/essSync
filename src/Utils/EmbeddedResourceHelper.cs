using System.Reflection;

public class EmbeddedResourceHelper
{
    public static Stream? GetEmbeddedResource(string resourcePath)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Resource names are: Namespace.folder.filename
        // Example: "essSync.dist.index.html"
        var fullResourceName = $"essSync.{resourcePath.Replace('/', '.').Replace('\\', '.')}";

        return assembly.GetManifestResourceStream(fullResourceName);
    }

    public static string GetEmbeddedResourceAsString(string resourcePath)
    {
        using var stream = GetEmbeddedResource(resourcePath);
        if (stream == null) return null;

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public static byte[] GetEmbeddedResourceAsBytes(string resourcePath)
    {
        using var stream = GetEmbeddedResource(resourcePath);
        if (stream == null) return null;

        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }
}