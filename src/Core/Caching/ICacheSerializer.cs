namespace DevEnterprise.Foundation.Caching
{
  public interface ICacheSerializer<TResult>
  {
    TResult Deserialize(string objectToDeserialize);

    string Serialize(TResult objectToSerialize);
  }
}
