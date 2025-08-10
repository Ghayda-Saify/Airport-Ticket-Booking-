using System.Text.Json;

namespace ce;

public class PassengerService
{
    #region Properties
    public Passenger Passenger{get;set;}
    public List<Flight> AllFlights{get; set; }
    public List<Booking> AllBookings { get; set; }
    #endregion

    #region private fields
    
    private readonly FileDataService _dataService;
    private int _bookingId = 0;
    private string _flightFilePath = "Data/Flights.json";
    private string _bookingFilePath = "Data/Booking.json";
    private JsonSerializerOptions _jsonOptions;
    
    #endregion

    public PassengerService()
    {
        _dataService = new FileDataService();
    }
    
    #region Load Data Files
    /// <summary>
    /// Asynchronously reads flight data from a JSON file and converts it into a List of Flight objects.
    /// </summary>
    /// <returns>A Task that represents the asynchronous operation. The task result contains a List of Flight objects, or an empty list if the file doesn't exist or is empty.</returns>
    private async Task LoadFlightsAndBookingFromFileAsync()
    {
        AllFlights =await _dataService.Read<Flight>(_flightFilePath);
        AllBookings =await _dataService.Read<Booking>(_bookingFilePath);

    }
    #endregion
    
    #region Search 
    /// <summary>
    /// Searches for available flights based on a flexible set of criteria.
    /// </summary>
    /// <param name="price"></param>
    /// <param name="departureCountry"></param>
    /// <param name="destinationCountry"></param>
    /// <param name="DepartureDate"></param>
    /// <param name="departureAirport"></param>
    /// <param name="arrivalAirport"></param>
    /// <param name="flightClass"></param>
    /// <returns></returns>
    public async Task<List<Flight>> SearchFlights(double price, string departureCountry, string destinationCountry,DateTime DepartureDate,string departureAirport = "" , string arrivalAirport = "",ClassType flightClass = ClassType.Economy)
    {
        await LoadFlightsAndBookingFromFileAsync();
        
            List<Flight> result = AllFlights.Where(f =>
                (f.Price <= price && f.DepartureCountry.Equals(departureCountry) &&
                 f.DestinationCountry.Equals(destinationCountry)
                 && f.DepartureDate.Equals(DepartureDate) && f.DepartureAirport.Contains(departureAirport) &&
                 f.ArrivalAirport.Contains(arrivalAirport))).ToList() ?? new List<Flight>();
            if (flightClass == ClassType.Business)
            {
                result = result.Where(f => f.BusinessPrice <= price).ToList();
            }
            else if (flightClass == ClassType.FirstClass)
            {
                result = result.Where(f => f.FirstClassPrice <= price).ToList();
            }

            Console.WriteLine($"Found {result.Count} flights");
            return result;
    }
        
    
    #endregion
    
    #region Book a Flight
    /// <summary>
    /// let the user book a flight
    /// </summary>
    /// <param name="flight"></param>
    /// <param name="passenger"></param>
    public async Task BookAFlight(int flightId, ClassType flightClass)
    {
        await LoadFlightsAndBookingFromFileAsync();
        var flight = AllFlights.SingleOrDefault(f => f.FlightID == flightId);
        if (flight == null)
        {
            Console.WriteLine("Error: Flight not found");
            return;
        }
        Booking booking = new Booking();
        Booking lastBooking = AllBookings.OrderBy(b => b.BookingID).Last();
        booking.BookingID = lastBooking.BookingID++;
        booking.FlightId = flight.FlightID;
        booking.PassengerId = Passenger.PassengerId;
        booking.FlightClass = flightClass;
        //store in json file
        AllBookings.Add(booking);
        await _dataService.Write(_bookingFilePath, AllBookings);

    }
    #endregion

    #region  Cancel Flight
    /// <summary>
    /// To cancel the booking based on booking id
    /// </summary>
    /// <param name="bookingId"></param>
    public async Task CancelBooking()
    {
        await LoadFlightsAndBookingFromFileAsync();
        Booking booking = AllBookings.SingleOrDefault(b => b.PassengerId == Passenger.PassengerId);
        if (booking == null)
        {
            Console.WriteLine("Booking not found. Please make sure your Booking ID is correct.");
        }
        else
        {
            AllBookings.Remove(booking);
            await _dataService.Write<Booking>(_bookingFilePath, AllBookings);
        }
    }
    #endregion
    
    #region  Modify Booking
   /// <summary>
   /// To Modify Booking
   /// </summary>
   /// <param name="flightClass"></param>
    public async Task ModifyBooking( ClassType flightClass)
    {
        await LoadFlightsAndBookingFromFileAsync();
        Booking booking = AllBookings.SingleOrDefault(b => b.PassengerId == Passenger.PassengerId);
        if (booking == null)
        {
            Console.WriteLine("Booking not found. Please make sure your Booking ID is correct.");
        }
        else
        {
            AllBookings.Remove(booking);
            booking.FlightClass = flightClass;
            AllBookings.Add(booking);
            await _dataService.Write<Booking>(_bookingFilePath, AllBookings);
        }
    }
   #endregion
   
   #region  View Flight's info
    /// <summary>
    /// View the Booking
    /// </summary>
    /// <returns></returns>
    public async Task<List<Booking>> ViewPersonalBooking()
    {
        await LoadFlightsAndBookingFromFileAsync();
        try
        {
            var booking = AllBookings.Where(b => b.PassengerId == Passenger.PassengerId).ToList();
            if (booking == null)
            {
                Console.WriteLine("Booking not found. Seems like you don't have booked any fight yet!");
                return new  List<Booking>();
            }
            else
            {
                return booking;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new  List<Booking>();
        }

    }
    #endregion
}