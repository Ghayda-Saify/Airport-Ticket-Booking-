namespace ce;

public class Passenger
{
    public int PassengerId{get;set;}
    public string Name{get;set;}

    public Passenger(string Name)
    {
        this.Name = Name;
        //TODO : get all passingers to a list then take the lastest ID, inc it and assign it to the passenger
    }
}