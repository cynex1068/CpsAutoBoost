namespace WindowsCpsTool;

enum AppLanguage
{
    System,
    English,
    French,
    German,
    Russian,
    Ukrainian,
    Chinese,
    Japanese
}

sealed record LanguageChoice(AppLanguage Language, string Name)
{
    public override string ToString() => Name;
}

sealed record UiText(
    string Language,
    string Enable,
    string Hint,
    string LeftThreshold,
    string RightThreshold,
    string LeftMultiplier,
    string RightMultiplier,
    string Button,
    string RealCps,
    string TotalCps,
    string Left,
    string Right,
    string Formula,
    string Running,
    string Stopped);
