namespace ce;

public class Booking
{
    public int BookingID{get;set;}
    public int FlightId { get; set; }
    public int PassengerId { get; set; }
    public Flight.Class FlightClass { get; set; }
    public double PricePaid { get; set; }

}