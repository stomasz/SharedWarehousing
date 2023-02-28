namespace SharedWarehousingCore.Helpers;

public class BasicException : Exception
{
    public BasicException() : base("Błąd")
    {
    }

    public BasicException(string message) : base(message)
    {
    }
}