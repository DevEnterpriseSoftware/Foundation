using Polly.Caching;
using System.Text.Json;

namespace DevEnterprise.Foundation.Caching.Serialization
{
  public sealed class JsonSerializer<TResult> : ICacheSerializer<TResult>, ICacheItemSerializer<TResult, string>
  {
    private readonly JsonSerializerOptions serializerOptions;

    public JsonSerializer() : this(null)
    {
    }

    public JsonSerializer(JsonSerializerOptions serializerOptions)
    {
      this.serializerOptions = serializerOptions ?? SerializerOptions.DefaultJsonSerializerOptions;
    }

    public TResult Deserialize(string objectToDeserialize) => JsonSerializer.Deserialize<TResult>(objectToDeserialize, serializerOptions);

    public string Serialize(TResult objectToSerialize) => JsonSerializer.Serialize(objectToSerialize, serializerOptions);
  }
}
