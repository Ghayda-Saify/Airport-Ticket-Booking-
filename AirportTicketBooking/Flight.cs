namespace ce;

public class Flight
{
    public int FlightID { get; set; }
    public string DepartureCountry { get; set; }
    public string DestinationCountry { get; set; }
    public DateTime DepartureDate { get; set; }
    public string DepartureAirport  { get; set; }
    public string ArrivalAirport {get; set;}
    public decimal EconomyPrice{get;set;}
    public decimal BusinessPrice{get;set;}
    public decimal FirstClassPrice{get;set;}
    
}