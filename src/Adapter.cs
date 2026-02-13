namespace MetaFrm.Database
{
    /// <summary>
    /// Database Adapter 클래스 입니다.
    /// </summary>
    public class Adapter : IAdapter
    {
        string[] IAdapter.ConnectionNames()
        {
            return this.GetAttribute("Providers").Split(',');
        }

        IDatabase IAdapter.CreateDatabase()
        {
            return ((IAdapter)this).CreateDatabase("");
        }

        IDatabase IAdapter.CreateDatabase(string connectionName)
        {
            string commandTimeout;
            string databaseNamespace;
            string connectionString;

            if (!string.IsNullOrEmpty(connectionName))
                connectionName = $".{connectionName}";

            databaseNamespace = this.GetAttribute($"Provider{connectionName}");

            if (string.IsNullOrEmpty(databaseNamespace))
                throw new MetaFrmException($"데이터 베이스 Provider{connectionName}가 없습니다.");

            connectionString = this.GetAttribute($"ConnectionString{connectionName}");

            if (string.IsNullOrEmpty(connectionString))
                throw new MetaFrmException($"연결 문자열 ConnectionString{connectionName}가 없습니다.");

            commandTimeout = this.GetAttribute("CommandTimeout");

            commandTimeout ??= "60000";

            return ((IAdapter)this).CreateDatabase(databaseNamespace, connectionString, commandTimeout.ToInt());
        }

        IDatabase IAdapter.CreateDatabase(string providerNamespace, string connectionString, int commandTimeout)
        {
            IDatabase? database;

            database = (IDatabase?)Factory.CreateInstance(providerNamespace, false, true) ?? throw new MetaFrmException("CreateDatabase fail");
            database.Connection.ConnectionString = connectionString;
            database.Command.CommandTimeout = commandTimeout;

            //database = new Database.Influx();
            //database = (IDatabase)Factory.CreateInstance(@"E:\Work\Project\Atomus\Database\MySQL\bin\Debug\Atomus.Database.MySQL.dll", "Atomus.Database.MySQL", false, true);

            return database;
        }
    }
}