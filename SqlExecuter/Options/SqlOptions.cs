namespace SqlExecuter.Options
{
    class SqlOptions
    {

        public string ConnectionString { get; set; } = string.Empty;

        public bool StopOnError { get; set; } = false;
    }
}
