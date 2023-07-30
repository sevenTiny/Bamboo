namespace Test.Common
{
    public class ConnectionStringHelper
    {
        public static string ConnectionString_Write_MySql = "server=192.168.0.107;Port=39901;database=SevenTinyTest;uid=root;pwd=123456;Allow User Variables=true;SslMode=none;";
        public static string[] ConnectionStrings_Read_MySql = new[] { ConnectionString_Write_MySql, ConnectionString_Write_MySql, ConnectionString_Write_MySql };

        public static string ConnectionString_Write_SqlServer = "Data Source=127.0.0.1;Initial Catalog=SevenTinyTest;User ID=sa;Password=123456;MultipleActiveResultSets=True;";
        public static string[] ConnectionStrings_Read_SqlServer = new[] { ConnectionString_Write_SqlServer, ConnectionString_Write_SqlServer, ConnectionString_Write_SqlServer };

        public static string ConnectionString_Write_MongoDb = "127.0.0.1";
    }
}
