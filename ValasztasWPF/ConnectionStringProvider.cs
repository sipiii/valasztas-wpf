using System.IO;

namespace ValasztasWPF
{
    public static class ConnectionStringProvider
    {
        private static readonly string PreferencesPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Preferences",
            "MySQLConnection.txt"
        );

        public static string GetConnectionString()
        {
            if (!File.Exists(PreferencesPath))
                throw new FileNotFoundException(
                    $"A kapcsolati beállítások fájl nem található: {PreferencesPath}");

            return File.ReadAllText(PreferencesPath).Trim();
        }
    }
}
