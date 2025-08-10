using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ce;

//Custom Validation Attribute
public class FutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value is DateTime dateTime)
        {
            return dateTime > DateTime.Now;
        }
        return false;
    }
}
public class Flight
{
    [Key]
    [Range(1, int.MaxValue, ErrorMessage = "Flight ID must be a positive number.")]
    public int FlightID { get; set; }
    [Required]
    public string DepartureCountry { get; set; }
    [Required]
    public string DestinationCountry { get; set; }
    
    [FutureDate(ErrorMessage = "Departure date must be in the future.")]
    public DateTime DepartureDate { get; set; }
    public string DepartureAirport  { get; set; }
    public string ArrivalAirport {get; set;}
    [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
    [DataType(DataType.Currency)]
    public double EconomyPrice { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
    [DataType(DataType.Currency)]
    public double BusinessPrice { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
    [DataType(DataType.Currency)]
    public double FirstClassPrice { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
    [DataType(DataType.Currency)]
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
        sb.AppendLine("Economy Price: " + EconomyPrice);
        sb.AppendLine("Business Price: " + BusinessPrice);
        sb.AppendLine("First Class Price: " + FirstClassPrice);
        sb.AppendLine("------------------------------------------------");
        return sb.ToString();
    }
}