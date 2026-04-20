namespace BLL.Dtos;

public record PlayerScoreDto(
    string Username,
    int MatchesPlayed,
    int Wins,
    int Losses,
    int Draws,
    double Score);
