namespace Domain.Exceptions;

public abstract class ChessTournamentException : Exception
{
    protected ChessTournamentException(string message) : base(message) { }
    protected ChessTournamentException(string message, Exception inner) : base(message, inner) { }
}

public sealed class NotFoundException : ChessTournamentException
{
    public NotFoundException(string message) : base(message) { }
}

public sealed class ConflictException : ChessTournamentException
{
    public ConflictException(string message) : base(message) { }
    public ConflictException(string message, Exception inner) : base(message, inner) { }
}

public sealed class ValidationException : ChessTournamentException
{
    public ValidationException(string message) : base(message) { }
    public ValidationException(string message, Exception inner) : base(message, inner) { }
}
