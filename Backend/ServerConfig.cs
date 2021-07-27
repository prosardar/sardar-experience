namespace Backend
{
    /// <summary>
    /// Класс с настройками сервера
    /// </summary>
    public class ServerConfig
    {
        /// <summary>
        /// Наименование тега в файле с настройками
        /// </summary>
        public const string TagName = "AppServerSetting";

        public int Port { get; set; }

        public ServerConfig()
        {

        }
    }
}