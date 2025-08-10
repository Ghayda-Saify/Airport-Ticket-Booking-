using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using CsvHelper;
using System.Reflection;

namespace ce;

public class ManagerService
{
    public List<Passenger> PassengerList { get; set; }
    public List<Booking> BookingList { get; set; }
    public List<Flight> FlightList { get; set; }
    private readonly FileDataService _fileDataService;
    private readonly string _flightspath = "Data/flights.json";
    private readonly string _bookingspath =  "Data/Bookings.json";
    private readonly string _passengerspath  = "Data/Passengers.json";

    public ManagerService()
    {
        _fileDataService = new FileDataService();
    }

    #region  Filter/// <summary>
    /// Filters bookings based on a combination of criteria.
    /// </summary>
    /// <param name="parameter">The filter criteria.</param>
    /// <returns>A list of detailed booking information that matches all criteria.</returns>
    public async Task Filter(BookingFilterParameters parameter)
    {   
        
        PassengerList = await _fileDataService.Read<Passenger>(_passengerspath);
        BookingList = await _fileDataService.Read<Booking>(_bookingspath);
        FlightList = await _fileDataService.Read<Flight>(_flightspath);
        
        var query = (from b in BookingList
                     join f in FlightList on b.FlightId equals f.FlightID
                     join p in PassengerList on b.PassengerId equals p.PassengerId
                     select new { Booking = b, Flight = f, Passenger = p }).AsQueryable();

        //1
        if (parameter.FlightId.HasValue)
        {
            query = query.Where(x => x.Flight.FlightID == parameter.FlightId.Value);
        }
        //2
        if (parameter.PassengerId.HasValue)
        {
            query = query.Where(x => x.Passenger.PassengerId == parameter.PassengerId.Value);
        }
        //3
        if (parameter.FlightClass.HasValue)
        {
            query = query.Where(x => x.Booking.FlightClass == parameter.FlightClass.Value);
        }
        //4
        if (!string.IsNullOrEmpty(parameter.DepartureCountry))
        {
            query = query.Where(x => x.Flight.DepartureCountry.Equals(parameter.DepartureCountry, StringComparison.OrdinalIgnoreCase));
        }
        //5
        if (!string.IsNullOrEmpty(parameter.DestinationCountry))
        {
            query = query.Where(x => x.Flight.DestinationCountry.Equals(parameter.DestinationCountry, StringComparison.OrdinalIgnoreCase));
        }
        //6
        if (parameter.DepartureDate.HasValue)
        {
            query = query.Where(x => x.Flight.DepartureDate.Equals(parameter.DepartureDate));
        }
        //7
        if (!string.IsNullOrEmpty(parameter.DepartureAirport))
        {
            query = query.Where(x => x.Flight.DepartureAirport.Equals(parameter.DepartureAirport, StringComparison.OrdinalIgnoreCase));
        }
        //8
        if (!string.IsNullOrEmpty(parameter.ArrivalAirport))
        {
            query = query.Where(x => x.Flight.ArrivalAirport.Equals(parameter.ArrivalAirport, StringComparison.OrdinalIgnoreCase));
        }
        //9
        if (parameter.Price.HasValue)
        {
            query = query.Where(x => x.Flight.Price.Equals(parameter.Price));
        }
        var result = query.Select(item => new
        {
            PassengerId = item.Booking.PassengerId,
            PassengerName = item.Passenger.Name,
            FlightId = item.Flight.FlightID,
            BookingId = item.Booking.BookingID,
            Departure = item.Flight.DepartureCountry,
            Destination = item.Flight.DestinationCountry,
            Date = item.Flight.DepartureDate,
            BookingClass = item.Booking.FlightClass,
            Price = (item.Booking.FlightClass == ClassType.Economy) ? item.Flight.EconomyPrice : (item.Booking.FlightClass == ClassType.Business) ? item.Flight.BusinessPrice : item.Flight.FirstClassPrice,
        }).ToList();
        foreach (var item in result)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("PassengerId : "+item.PassengerId.ToString());
            sb.AppendLine("PassengerName : "+item.PassengerName);
            sb.AppendLine("FlightId : "+item.FlightId.ToString());
            sb.AppendLine("BookingId : "+item.BookingId.ToString());
            sb.AppendLine("Departure : "+item.Departure);
            sb.AppendLine("Destination : "+item.Destination);
            sb.AppendLine("Date : "+item.Date.ToString());
            sb.AppendLine("BookingClass : "+item.BookingClass.ToString());
            sb.AppendLine("Price : "+item.Price.ToString());
        }

      
    }
        #endregion
        
        #region UploadListOfFlights
        /// <summary>
        /// Upload List Of Flights If Valid
        /// </summary>
        /// <param name="csvFilePath"></param>
        /// <returns></returns>
        public async Task<List<string>> UploadListOfFlights(string csvFilePath)
        {
            var validationErrors = new List<string>();
            var newFlights = new List<Flight>();
            //Read CVS
            try
            {
                var reader = new StreamReader(csvFilePath);
                var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                newFlights = csv.GetRecords<Flight>().ToList();
               
            }
            catch (Exception ex)
            {
                return new List<string> { $"Failed to read CSV file: {ex.Message}" };

            }
            //validate the new data before adding any thing
            for (int i =0;i<newFlights.Count;i++)
            {
                var flight = newFlights[i];
                var validationContext = new ValidationContext(flight);
                var results = new List<ValidationResult>();
                if (!Validator.TryValidateObject(flight, validationContext, results, validateAllProperties: true))
                {
                    foreach (var result in results)
                    {
                        validationErrors.Add($"Row {i + 2}: {result.ErrorMessage}");
                    }
                }
                Console.WriteLine(flight);
            }
            //if there is any errors:
            if (validationErrors.Any())
            {
                return validationErrors;
            }
            //if there is no errors -> add all new flights to the main file
            FlightList =await _fileDataService.Read<Flight>(_flightspath);
            FlightList.AddRange(newFlights);
            await _fileDataService.Write(_flightspath,FlightList);
            return new List<string>();
    }
    #endregion


    #region Using Refliction in Validation
    public List<string> GetFlightValidationRules()
    {
        var rules = new List<string>();
        var properties  = typeof(Flight).GetProperties();
        foreach (var p in properties)
        {
            var attributes = p.GetCustomAttributes<ValidationAttribute>(true);
            foreach (var a in attributes)
            {
                StringBuilder sb =new StringBuilder();
                sb.AppendLine($"{p.Name} :");
                switch (a)
                {
                    case RequiredAttribute:
                        sb.AppendLine("Required");
                        break;
                    case FutureDateAttribute:
                        sb.AppendLine("The Date should be in the Future");
                        break;
                    case RangeAttribute range:
                        sb.AppendLine($"The value must be between {range.Minimum} and {range.Maximum}.");
                        break;
                    default:
                        sb.AppendLine(a.GetType().Name);
                        break;
                }
                rules.Add(sb.ToString());
            }
        }
        


        return rules;
    }
    #endregion

}

public class BookingFilterParameters
{
    public int? FlightId { get; set; }
    public int? PassengerId { get; set; }
    public ClassType? FlightClass { get; set; }
    public double? Price {get; set;}
    public string? DepartureCountry{get; set;}
    public string? DestinationCountry{get; set;}
    public DateTime? DepartureDate{get; set;}
    public string? DepartureAirport{get; set;}
    public string? ArrivalAirport{get; set;}
}