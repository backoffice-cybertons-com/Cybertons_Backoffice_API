using Npgsql;
using System;
using System.IO;

class Program
{
    static void Main()
    {
        string sqlPath = @"..\SoftBook.Infrastructure\sp_Master_Universal_Postgres.sql";
        if (!File.Exists(sqlPath))
        {
            Console.WriteLine($"Error: SQL file not found at {sqlPath}");
            return;
        }

        string sql = File.ReadAllText(sqlPath);
        string[] databases = { "softaccdb_1", "companymaster" }; // Updated to target the correct DB

        foreach (var db in databases)
        {
            try
            {
                string connectionString = $"Host=localhost;Database={db};Username=postgres;Password=Soft123";
                Console.WriteLine($"Connecting to {db}...");
                
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    Console.WriteLine($"Connected to {db}. Executing script...");
                    
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.ExecuteNonQuery();
                        Console.WriteLine($"SUCCESS: Script executed against {db}.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed for {db}: {ex.Message}");
            }
        }
        
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
