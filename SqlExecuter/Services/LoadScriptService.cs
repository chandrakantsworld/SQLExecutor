using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SqlExecuter.Services
{
    class LoadScriptService
    {
        private readonly Options.FileOptions fileOptions;

        public LoadScriptService(Options.FileOptions fileOptions)
        {
            this.fileOptions = fileOptions ?? throw new ArgumentNullException(nameof(fileOptions));
        }


        public IEnumerable<string> GetSqlCommands()
        {
            List<string> commands = new List<string>();

            try
            {
                DirectoryInfo d = new DirectoryInfo(fileOptions.DirectoryPath);
                FileInfo[] Files = d.GetFiles("*.sql");
                Console.WriteLine($"{Files.Length} file(s) found.");
                foreach (var file in Files)
                {

                    // Read the file contents
                    string fileContents = File.ReadAllText(file.FullName);

                    // Check to see if we are to split the commands
                    if (fileOptions.SplitOnGo)
                    {
                        // Loop over each split, adding the command to the list
                        foreach (string command in Regex.Split(fileContents, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled))
                        {
                            commands.Add(command);
                        }
                    }
                    else
                    {
                        // Just add the entire script as a single command
                        commands.Add(fileContents);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occured reading the file:\n{ex.Message}");
            }

            // Return the commands
            return commands as IEnumerable<string>;
        }
    }
}
