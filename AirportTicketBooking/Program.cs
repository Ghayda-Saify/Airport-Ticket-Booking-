﻿

using ce;

public class Program
{
    public async static Task<int> Main(string[] args)
    {
        Passenger passenger = new Passenger("Ahmed");
        PassengerService passengerService = new PassengerService(passenger);
        var result =await passengerService.SearchFlights(250.00,"Palestine","Turkey",DateTime.Parse("2025-09-15T10:30:00"),"QAIA","IST",Flight.Class.Economy);
        foreach (var flight in result)
        {
            Console.WriteLine(flight);
        }

        return 0;
    }
}