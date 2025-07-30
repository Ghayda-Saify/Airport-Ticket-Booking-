namespace ce;

public class SearchParameters
{
    /*
- Flight
- Price
- Departure Country
- Destination Country
- Departure Date
- Departure Airport
- Arrival Airport
- Passenger
- Class
*/
    public int FlightID { get; set; }
    public decimal? MaxPrice { get; set; }
    public string DepartureCountry{get;set;}
    public string DestinationCountry{get;set;}
    public DateTime? DepartureDate{get;set;}
    public string DepartureAirport{get;set;}
    public string ArrivalAirport{get;set;}
    public int PassengerID{get;set;}
    public string PassengerName{get;set;}
    public string Class{get;set;}
}