using System.Text.Json;

namespace ce;

public class PassengerService
{
    private IEnumerable<Flight> _allFlights;
    /// <summary>
    /// Reads flight data from a JSON file and converts it into a List of Flight objects.
    /// </summary>
    /// <param name="filePath">The path to the flights.json file.</param>
    /// <returns>A List of Flight objects, or an empty list if the file doesn't exist or is empty.</returns>
    public async Task<List<Flight>> LoadFlightsFromFileAsync(string filePath)
    {
        // Check if the file exists 
        if (!File.Exists("Data/Flights.json"))
        {
            Console.WriteLine("Error: Flights data file not found.");
            return new List<Flight>(); //to avoid errors
        }

        //Read
        try
        {
            await using FileStream openStream = File.OpenRead(filePath);
            if (openStream.Length == 0)
            {
                return new List<Flight>();
            }
            List<Flight> flights = await JsonSerializer.DeserializeAsync<List<Flight>>(openStream);
            return flights ?? new List<Flight>();
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error deserializing JSON: {ex.Message}");
            return new List<Flight>();
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
            return new List<Flight>();
        }
    }

    /// <summary>
    /// Searches for available flights based on a flexible set of criteria.
    /// </summary>
    /// <param name="parameter"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task<List<Flight>> SearchFlights(decimal price, string departureCountry, string destinationCountry,DateTime DepartureDate,string departureAirport , string arrivalAirport,Flight.Class flightClass)
    {
        _allFlights = await LoadFlightsFromFileAsync("AirportTicketBooking/Data/Flights.json");
        
        List<Flight> result= _allFlights.Where(f=> f.Price == price && f.DepartureCountry.Equals(departureCountry) && f.DestinationCountry.Equals(destinationCountry)
        && f.DepartureDate.Equals(DepartureDate) && f.DepartureAirport.Equals(departureAirport) && f.ArrivalAirport.Equals(arrivalAirport) && f.FlightClass.Equals(flightClass)
        ).ToList();
        
        return result;
    }

    public static void Main(string[] args)
    {
        PassengerService p=new PassengerService();
        p.LoadFlightsFromFileAsync("./Flights.json");
    }
}