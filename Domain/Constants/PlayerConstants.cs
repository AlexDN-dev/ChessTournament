namespace Domain.Constants;

public static class PlayerConstants
{
    public const int DefaultElo = 1200;

    public const string PasswordRegex = @"^(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$";
    public const string PasswordRequirementsMessage =
        "Le mot de passe doit contenir au moins 8 caractères, un chiffre et un caractère spécial.";

    public const string GenderMale = "Homme";
    public const string GenderFemale = "Femme";

    public static readonly IReadOnlyCollection<string> AllowedGenders = new[] { GenderMale, GenderFemale };
}
