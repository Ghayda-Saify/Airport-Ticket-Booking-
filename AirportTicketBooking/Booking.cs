namespace ce;

public class Booking
{
    public int BookingID{get;set;}
    public int FlightId { get; set; }
    public int PassengerId { get; set; }
    public string FlightClass { get; set; }
    public decimal PricePaid { get; set; }

}