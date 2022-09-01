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

            if (connectionName != "")
                connectionName = string.Format(".{0}", connectionName);

            databaseNamespace = this.GetAttribute(string.Format("Provider{0}", connectionName));

            if (databaseNamespace == null)
                throw new MetaFrmException(string.Format("데이터 베이스 Provider{0}가 없습니다.", connectionName));

            connectionString = this.GetAttribute(string.Format("ConnectionString{0}", connectionName));

            if (connectionString.IsNullOrEmpty())
                throw new MetaFrmException(string.Format("연결 문자열 ConnectionString{0}가 없습니다.", connectionName));

            commandTimeout = this.GetAttribute("CommandTimeout");

            commandTimeout ??= "60000";

            return ((IAdapter)this).CreateDatabase(databaseNamespace, connectionString, commandTimeout.ToInt());
        }

        IDatabase IAdapter.CreateDatabase(string providerNamespace, string connectionString, int commandTimeout)
        {
            IDatabase? database;

            database = (IDatabase?)Factory.CreateInstance(providerNamespace, false, true);
            //database = new Database.Influx();
            //database = (IDatabase)Factory.CreateInstance(@"E:\Work\Project\Atomus\Database\MySQL\bin\Debug\Atomus.Database.MySQL.dll", "Atomus.Database.MySQL", false, true);

            if (database == null)
                throw new MetaFrmException("CreateDatabase fail");

            database.Connection.ConnectionString = connectionString;
            database.Command.CommandTimeout = commandTimeout;

            return database;
        }
    }
}