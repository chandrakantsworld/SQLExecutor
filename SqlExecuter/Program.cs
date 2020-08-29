using SqlExecuter.Options;
using SqlExecuter.Services;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace SqlExecuter
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                // Step in to the Asyncronis program
                await MainAsync();
               // main.Wait();
                Console.WriteLine();
                Console.WriteLine("Execution completed.");
                Console.Read();
            }
            catch (AggregateException aggEx)
            {
                foreach (Exception ex in aggEx.InnerExceptions)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                }
            }
        }
        static async Task MainAsync()
        {
            SqlOptions sqlOptions = new SqlOptions();
            FileOptions fileOptions = new FileOptions();

            // Load the SQL Options from file
            sqlOptions.ConnectionString = ConfigurationManager.ConnectionStrings["SqlExecuter"].ConnectionString;
            sqlOptions.StopOnError = Convert.ToBoolean(ConfigurationManager.AppSettings["StopOnError"]);

            // Set the command file and options
            fileOptions.DirectoryPath = $"{ System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)}\\{Convert.ToString(ConfigurationManager.AppSettings["DirectoryPath"])}";
            fileOptions.SplitOnGo = Convert.ToBoolean(ConfigurationManager.AppSettings["SplitOnGo"]);

            // Load the script and execution services
            LoadScriptService scriptService = new LoadScriptService(fileOptions);
            ExecuteCommandService commandService = new ExecuteCommandService(sqlOptions);

            // Execute the commands
            foreach (string command in scriptService.GetSqlCommands())
            {
                if (!string.IsNullOrEmpty(command))
                {
                    Console.WriteLine();
                    Console.WriteLine(command);

                    await commandService.ExecuteCommand(command);
                }
            }
        }
    }
}
