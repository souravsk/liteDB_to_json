using Npgsql;

namespace Alphatag_Game.Services
{
    public class DatabaseInsertionService
    {
        private readonly string _connectionString;

        public DatabaseInsertionService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task InsertDataToPostgresAsync(string tableName, string json)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Insert the JSON data into the specified table
                    using (var command = new NpgsqlCommand($"INSERT INTO {tableName} (data) VALUES (@json)", connection))
                    {
                        command.Parameters.AddWithValue("@json", json);
                        await command.ExecuteNonQueryAsync();
                    }

                    Console.WriteLine($"Data inserted into table: {tableName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting data into database: {ex.Message}");
            }
        }
    }
}