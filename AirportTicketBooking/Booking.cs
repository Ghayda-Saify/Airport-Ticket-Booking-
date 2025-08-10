using System.Text;

namespace ce;

public class Booking
{
    public int BookingID{get;set;}
    public int FlightId { get; set; }
    public int PassengerId { get; set; }
    public ClassType FlightClass { get; set; }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Booking ID: {BookingID}");
        sb.AppendLine($"Flight ID: {FlightId}");
        sb.AppendLine($"Passenger ID: {PassengerId}");
        sb.AppendLine($"Class: {FlightClass}");
        return sb.ToString();
    }
}