namespace SimplyView
{
    public abstract record Setting(string Name) { }

    public record BoolSetting(string Name, bool Value) : Setting(Name) { }

    public record StringSetting(string Name, string Value) : Setting(Name) { }
}
