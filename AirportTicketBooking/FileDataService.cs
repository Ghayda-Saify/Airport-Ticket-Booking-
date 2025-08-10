using System.Text.Json;
using System.Text.Json.Serialization;

namespace ce;

public class FileDataService
{
    private JsonSerializerOptions _jsonSerializerOptions;
    public async Task<List<T>> Read<T>(string filePath)
    {
         // Check if the file exists before trying to read it.
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Error: data file not found.");
            return new List<T>();
        }

        try
        {
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                // This allows property names in JSON (like "flightID") to map to C# properties (like "FlightID").
                PropertyNameCaseInsensitive = true, 
                // This tells the deserializer to convert string values to enums.
                Converters = { new JsonStringEnumConverter() } 
            };

            // Open a stream to the file for efficient reading.
            await using FileStream openFileStream = File.OpenRead(filePath);

            // If the file is empty, return an empty list.
            if (openFileStream.Length == 0)
            {
                return new List<T>();
            }

            // Asynchronously deserialize the JSON stream into a List<Flight>, using our custom options.
            var DataAsList = await JsonSerializer.DeserializeAsync<List<T>>(openFileStream, _jsonSerializerOptions)  ?? new List<T>();
            return DataAsList;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error deserializing JSON: {ex.Message}");
            return new List<T>();
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
            return new List<T>();
        }
    }

    public async Task Write<T>(string filePath, List<T> data)
    {
        try
        {
            // 1. Serialize your list into a JSON string in memory.
            string jsonString = JsonSerializer.Serialize(data, _jsonSerializerOptions);

            // 2. Write that entire string to the file, automatically handling overwriting.
            await File.WriteAllTextAsync(filePath, jsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to file: {ex.Message}");
            
        }
        
    }
}