using System.Text;

namespace ce;

public class Flight
{
    public enum Class {Economy,Business,FirstClass}
    public int FlightID { get; set; }
    public string DepartureCountry { get; set; }
    public string DestinationCountry { get; set; }
    public DateTime DepartureDate { get; set; }
    public string DepartureAirport  { get; set; }
    public string ArrivalAirport {get; set;}
    public Class FlightClass { get; set; }
    public double Price { get; set; }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Flight ID: " + FlightID);
        sb.AppendLine("Departure Country: " + DepartureCountry);
        sb.AppendLine("Destination Country: " + DestinationCountry);
        sb.AppendLine("Departure Date: " + DepartureDate);
        sb.AppendLine("Departure Airport: " + DepartureAirport);
        sb.AppendLine("Arrival Airport: " + ArrivalAirport);
        sb.AppendLine("Flight Class: " + FlightClass);
        sb.AppendLine("Price: " + Price);
        sb.AppendLine("------------------------------------------------");
        return sb.ToString();
    }
}