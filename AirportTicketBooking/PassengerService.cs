using System.Text.Json;
using System.Text.Json.Serialization;

namespace ce;

public class PassengerService
{
    //proparities
    private IEnumerable<Flight> _allFlights;
    private int _bookingID = 0;
    public Passenger Passenger{get;set;}
    
    //Constructor
    public PassengerService(Passenger passenger)
    {
        Passenger = passenger;
    }
    
    /// <summary>
    /// Asynchronously reads flight data from a JSON file and converts it into a List of Flight objects.
    /// </summary>
    /// <returns>A Task that represents the asynchronous operation. The task result contains a List of Flight objects, or an empty list if the file doesn't exist or is empty.</returns>
    public async Task<List<Flight>> LoadFlightsFromFileAsync()
    {
        string filePath = "Data/Flights.json";
        // Check if the file exists before trying to read it.
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Error: Flights data file not found.");
            return new List<Flight>(); // Return an empty list to avoid errors.
        }

        try
        {
            var options = new JsonSerializerOptions
            {
                // This allows property names in JSON (like "flightID") to map to C# properties (like "FlightID").
                PropertyNameCaseInsensitive = true, 
                // This tells the deserializer to convert string values to enums.
                Converters = { new JsonStringEnumConverter() } 
            };

            // Open a stream to the file for efficient reading.
            await using FileStream openStream = File.OpenRead(filePath);

            // If the file is empty, return an empty list.
            if (openStream.Length == 0)
            {
                return new List<Flight>();
            }

            // Asynchronously deserialize the JSON stream into a List<Flight>, using our custom options.
            List<Flight> flights = await JsonSerializer.DeserializeAsync<List<Flight>>(openStream, options);
            return flights ?? new List<Flight>();
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error deserializing JSON: {ex.Message}");
            return new List<Flight>(); // Return an empty list on error.
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
            return new List<Flight>();
        }
    }
    /// <summary>
    /// First Feature
    /// Searches for available flights based on a flexible set of criteria.
    /// </summary>
    /// <param name="parameter"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task<List<Flight>> SearchFlights(double price, string departureCountry, string destinationCountry,DateTime DepartureDate,string departureAirport = "QAIA" , string arrivalAirport = "IST",Flight.Class flightClass= Flight.Class.Economy)
    {
        _allFlights = await LoadFlightsFromFileAsync();
        
        List<Flight> result= _allFlights.Where(f=> f.Price <= price && f.DepartureCountry.Equals(departureCountry) && f.DestinationCountry.Equals(destinationCountry)
        && f.DepartureDate.Equals(DepartureDate) && f.DepartureAirport.Equals(departureAirport) && f.ArrivalAirport.Equals(arrivalAirport) && f.FlightClass.Equals(flightClass)
        ).ToList();
        Console.WriteLine($"Found {result.Count} flights");
        return result;
    }

    public void BookFlight(Flight flight,Passenger passenger)
    {
        Booking booking = new Booking();
        booking.BookingID = _bookingID;
        _bookingID++;
        booking.FlightId = flight.FlightID;
        booking.FlightClass = flight.FlightClass;
        booking.PassengerId = passenger.PassengerId;
        booking.PricePaid = flight.Price;
        //sort in json file
        //TODO
    }

    public void CancelBooking(Booking booking)
    {
        
    }

    public void ModifyBooking(Booking booking)
    {
        
    }

    public void ViewPersonalBooking(Booking booking)
    {
        
    }
   
}