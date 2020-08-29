using SqlExecuter.Options;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SqlExecuter.Services
{
    class ExecuteCommandService
    {
        private readonly SqlOptions sqlOptions;
        private readonly DbProviderFactory dbProvider = SqlClientFactory.Instance;

        public ExecuteCommandService(SqlOptions sqlOptions)
        {
            this.sqlOptions = sqlOptions ?? throw new ArgumentNullException(nameof(sqlOptions));
        }


        public async Task ExecuteCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
            {
                throw new ArgumentNullException(nameof(command));
            }

            try
            {
                using (DbConnection conn = dbProvider.CreateConnection())
                {
                    using (DbCommand cmd = conn.CreateCommand())
                    {
                        // Base line the command
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = command;

                        // Open and Execute
                        conn.ConnectionString = sqlOptions.ConnectionString;
                        await conn.OpenAsync();
                        var count = await cmd.ExecuteNonQueryAsync();
                        Console.WriteLine($"{count} rows affected.");
                    }
                }
            }
            catch (DbException dbEx)
            {
                // Output the error in red, resetting the console colour once finished
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(dbEx.Message);
                Console.ResetColor();
               
                if (sqlOptions.StopOnError)
                {
                    throw new Exception($"Stopping execution due to errors:\n{dbEx.Message}");
                }
            }
        }
    }
}
