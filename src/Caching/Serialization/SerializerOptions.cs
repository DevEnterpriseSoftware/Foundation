using System.Text.Json;

namespace DevEnterprise.Foundation.Caching.Serialization
{
  public static class SerializerOptions
  {
    public static JsonSerializerOptions DefaultJsonSerializerOptions { get; } = new JsonSerializerOptions()
    {
      WriteIndented = false,
      AllowTrailingCommas = false,
      IgnoreNullValues = true,
      ReadCommentHandling = JsonCommentHandling.Skip,
      PropertyNamingPolicy = null
    };
  }
}
