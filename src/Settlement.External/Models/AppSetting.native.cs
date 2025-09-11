namespace Settlement.External.Models;

public class AppSetting
{
    public bool EnableLogRequest { get; set; }

    public string SecurityToken { get; init; }

    public string EnvironmentName { get; set; }
}