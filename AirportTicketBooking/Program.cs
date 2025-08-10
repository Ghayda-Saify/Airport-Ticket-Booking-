using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// NOTE: Make sure this namespace matches your project's namespace
namespace ce
{
    class Program
    {
        // Instantiate services once. The PassengerService will be updated with the logged-in passenger.
        private static readonly PassengerService _passengerService = new PassengerService();
        private static readonly ManagerService _managerService = new ManagerService();

        // Main entry point of the application
        static async Task Main(string[] args)
        {
            // Ensure data directory exists to prevent errors on first run
            Directory.CreateDirectory("Data");

            while (true)
            {
                Console.Clear();
                Console.WriteLine("======================================");
                Console.WriteLine("  Welcome to the Airline Booking System");
                Console.WriteLine("======================================");
                Console.WriteLine("\nAre you a passenger or a manager?");
                Console.WriteLine("  1. Passenger");
                Console.WriteLine("  2. Manager");
                Console.WriteLine("  3. Exit");
                Console.Write("\nPlease select your role: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        await HandlePassengerMenu();
                        break;
                    case "2":
                        await HandleManagerMenu();
                        break;
                    case "3":
                        Console.WriteLine("\nThank you for using the system. Goodbye!");
                        return; // Exit the application
                    default:
                        Console.WriteLine("Invalid option. Please press any key to try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        #region Passenger UI
        private static async Task HandlePassengerMenu()
        {
            // --- Passenger "Login" ---
            // Your service depends on having a Passenger object set.
            // In a real app, this would involve a database lookup. Here, we simulate it.
            Console.Write("\nPlease enter your Passenger ID to begin: ");
            if (!int.TryParse(Console.ReadLine(), out int passengerId) || passengerId <= 0)
            {
                Console.WriteLine("Invalid ID format. Returning to main menu.");
                Console.ReadKey();
                return;
            }
            // Set the passenger for this session.
            _passengerService.Passenger = new Passenger { PassengerId = passengerId, Name = "Test Passenger" };


            while (true)
            {
                Console.Clear();
                Console.WriteLine($"--- Passenger Menu (Logged in as Passenger ID: {passengerId}) ---");
                Console.WriteLine("  1. Search for Flights");
                Console.WriteLine("  2. Book a Flight");
                Console.WriteLine("  3. View My Booking");
                Console.WriteLine("  4. Cancel My Booking");
                Console.WriteLine("  5. Modify My Booking");
                Console.WriteLine("  6. Back to Main Menu");
                Console.Write("\nSelect an option: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        await SearchFlightsUI();
                        break;
                    case "2":
                        await BookFlightUI();
                        break;
                    case "3":
                        await ViewMyBookingUI();
                        break;
                    case "4":
                        await CancelBookingUI();
                        break;
                    case "5":
                        await ModifyBookingUI();
                        break;
                    case "6":
                        return; // Go back to the main menu
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        private static async Task SearchFlightsUI()
        {
            Console.Clear();
            Console.WriteLine("--- Search for Flights ---");
            Console.WriteLine("Please enter your search criteria.");

            // Collect search parameters from the user
            Console.Write("Departure Country: ");
            string depCountry = Console.ReadLine();

            Console.Write("Destination Country: ");
            string destCountry = Console.ReadLine();

            Console.Write("Max Price: ");
            double.TryParse(Console.ReadLine(), out double price);

            Console.Write("Departure Date (yyyy-mm-dd): ");
            DateTime.TryParse(Console.ReadLine(), out DateTime date);

            var flights = await _passengerService.SearchFlights(price, depCountry, destCountry, date);

            if (flights == null || !flights.Any())
            {
                Console.WriteLine("\nNo flights found matching your criteria.");
                return;
            }

            Console.WriteLine("\n--- Available Flights ---");
            foreach (var flight in flights)
            {
                Console.WriteLine($"  Flight ID: {flight.FlightID}, From: {flight.DepartureAirport}, To: {flight.ArrivalAirport}, Date: {flight.DepartureDate:yyyy-MM-dd}, Price: {flight.EconomyPrice:C}");
            }
        }

        private static async Task BookFlightUI()
        {
            Console.Clear();
            Console.WriteLine("--- Book a Flight ---");
            Console.Write("Enter the Flight ID you wish to book: ");
            int flightId = int.Parse(Console.ReadLine());
            Console.Write("Enter the Class you wish to book: \n1. Economy\n2. Business\n3. FirstClass\n(Please enter the number of the class you select): ");
            int classNum =  int.Parse(Console.ReadLine());
            ClassType Class = classNum == 1 ? ClassType.Economy : classNum==2? ClassType.Business : ClassType.FirstClass;
            if (flightId>0)
            {
                // The service method handles success/error messages internally
                await _passengerService.BookAFlight(flightId,Class);
                Console.WriteLine("Booking process completed.");
            }
            else
            {
                Console.WriteLine("Invalid Flight ID Or Class Number.");
            }
        }

        private static async Task ViewMyBookingUI()
        {
            Console.Clear();
            Console.WriteLine("--- Your Booking Information ---");
            var booking = await _passengerService.ViewPersonalBooking();

            if (booking != null)
            {
                foreach (var book in booking)
                {
                    Console.WriteLine(book);       
                }
            }
        }

        private static async Task CancelBookingUI()
        {
            Console.Clear();
            Console.WriteLine("--- Cancel My Booking ---");
            Console.Write("Are you sure you want to cancel your booking? (y/n): ");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                // Your service method finds the booking by the logged-in passenger ID.
                await _passengerService.CancelBooking();
                Console.WriteLine("Cancellation process completed.");
            }
            else
            {
                Console.WriteLine("Cancellation aborted.");
            }
        }

        private static async Task ModifyBookingUI()
        {
            Console.Clear();
            Console.WriteLine("--- Modify My Booking ---");
            Console.Write("Enter new flight class (Economy, Business, FirstClass): ");
            if (Enum.TryParse<ClassType>(Console.ReadLine(), true, out ClassType newClass))
            {
                await _passengerService.ModifyBooking(newClass);
                Console.WriteLine("Modification process completed.");
            }
            else
            {
                Console.WriteLine("Invalid class type.");
            }
        }
        #endregion

        #region Manager UI
        private static async Task HandleManagerMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("--- Manager Menu ---");
                Console.WriteLine("  1. Filter All Bookings");
                Console.WriteLine("  2. Upload Flights from CSV");
                Console.WriteLine("  3. View Flight Validation Rules");
                Console.WriteLine("  4. Back to Main Menu");
                Console.Write("\nSelect an option: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        await FilterAllBookingsUI();
                        break;
                    case "2":
                        await UploadFlightsUI();
                        break;
                    case "3":
                        ViewValidationRulesUI();
                        break;
                    case "4":
                        return; // Go back to the main menu
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        private static async Task FilterAllBookingsUI()
        {
            Console.Clear();
            Console.WriteLine("--- Filter Bookings ---");
           

            var parameters = new BookingFilterParameters();

            Console.Write("Enter Flight ID to filter by (or press Enter to skip): ");
            string flightIdInput = Console.ReadLine();
            if (int.TryParse(flightIdInput, out int flightId))
            {
                parameters.FlightId = flightId;
            }

            Console.Write("Enter Passenger ID to filter by (or press Enter to skip): ");
            string passengerIdInput = Console.ReadLine();
            if (int.TryParse(passengerIdInput, out int pId))
            {
                parameters.PassengerId = pId;
            }
            Console.Write("Enter Class Type to filter by\n1. Economy\n2. Bussinus\n3. FirstClass (or press Enter to skip): ");
            string FlightClass = Console.ReadLine();
            if (int.TryParse(FlightClass, out int cId))
            {
                parameters.FlightClass = cId==1?ClassType.Economy: cId==2?ClassType.Business:ClassType.FirstClass;
            }
            Console.Write("Enter Max Price to filter by (or press Enter to skip): ");
            string Price = Console.ReadLine();
            if (double.TryParse(Price, out double PId))
            {
                parameters.Price = PId;
            }
            Console.Write("Enter Departure Country to filter by (or press Enter to skip): ");
            string DepartureCountry = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(DepartureCountry))
            {
                parameters.DepartureCountry = DepartureCountry;
            }Console.Write("Enter Destination Country to filter by (or press Enter to skip): ");
            string DestinationCountry = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(DestinationCountry))
            {
                parameters.DestinationCountry = DestinationCountry;
            }
            Console.Write("Enter DepartureDate (yyyy-mm-ddThh:mm:ss) to filter by (or press Enter to skip): ");
            string DepartureDate = Console.ReadLine();
            if (DateTime.TryParse(DepartureDate, out DateTime d))
            {
                parameters.DepartureDate = d;
            }
            Console.Write("Enter Departure Airport to filter by (or press Enter to skip): ");
            string DepartureAirport = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(DestinationCountry))
            {
                parameters.DepartureAirport = DepartureAirport;
            }
            Console.Write("Enter Arrival Airport to filter by (or press Enter to skip): ");
            string ArrivalAirport = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(ArrivalAirport))
            {
                parameters.ArrivalAirport = ArrivalAirport;
            }
            Console.WriteLine("\n--- Filter Results ---");
           
            await _managerService.Filter(parameters);
        }

        private static async Task UploadFlightsUI()
        {
            Console.Clear();
            Console.WriteLine("--- Upload Flights from CSV ---");
            Console.Write("Enter the full path to the CSV file: ");
            string filePath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                Console.WriteLine("\nError: File not found or path is empty.");
                return;
            }

            List<string> uploadErrors = await _managerService.UploadListOfFlights(filePath);

            if (uploadErrors.Any())
            {
                Console.WriteLine("\nUpload failed with the following errors:");
                foreach (var error in uploadErrors)
                {
                    Console.WriteLine($"  - {error}");
                }
            }
            else
            {
                Console.WriteLine("\nFlights uploaded successfully!");
            }
        }

        private static void ViewValidationRulesUI()
        {
            Console.Clear();
            Console.WriteLine("--- Flight Data Validation Rules ---");
            Console.WriteLine("These are the rules applied when uploading flights from a CSV file:\n");

            var rules = _managerService.GetFlightValidationRules();
            foreach (var rule in rules)
            {
                // The StringBuilder in your service adds extra newlines, so we use Write instead of WriteLine.
                Console.Write($"  - {rule}");
            }
        }
        #endregion
    }
}
