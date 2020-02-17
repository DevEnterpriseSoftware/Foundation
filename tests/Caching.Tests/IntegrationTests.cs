using StackExchange.Redis;
using System.Data.SqlClient;
using Xunit;


namespace Test
{
  public sealed class IntegrationTests
  {
    private const string RedisConnectionString = "127.0.0.1:6379,ssl=False,connectTimeout=3000";
    private const string SqlServerConnectionString = "Server=127.0.0.1;Database=master;Trusted_Connection=True;";
    private static readonly bool RedisConnectionAvailable;
    private static readonly bool SqlServerConnectionAvailable;

#pragma warning disable S3963 // "static" fields should be initialized inline
    static IntegrationTests()
    {
      try
      {
        // Try to connect to local Redis, if this fails the tests will be skipped.
        using (var redis = ConnectionMultiplexer.Connect(RedisConnectionString))
        {
          RedisConnectionAvailable = redis.IsConnected;
        }
      }
      catch (RedisConnectionException)
      {
        RedisConnectionAvailable = false;
      }

      try
      {
        using (var sql = new SqlConnection(SqlServerConnectionString))
        {
          sql.Open();
          SqlServerConnectionAvailable = true;
        }
      }
      catch (SqlException)
      {
        SqlServerConnectionAvailable = false;
      }
    }
#pragma warning restore S3963 // "static" fields should be initialized inline

    [SkippableFact]
    public void RedisSmokeTest()
    {
      Skip.IfNot(RedisConnectionAvailable, "Could not connect to local Redis service.");
    }


    [SkippableFact]
    public void SqlServerSmokeTest()
    {
      Skip.IfNot(SqlServerConnectionAvailable, "Could not connect to local SQL Server database.");
    }
  }
}