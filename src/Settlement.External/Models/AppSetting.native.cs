namespace Settlement.External.Models;

public class AppSetting
{
    public bool EnableLogRequest { get; set; }

    public string SecurityToken { get; init; }
}


public class RabbitMqSetting
{
    public const string SectionName = "RabbitMQ";
    public string VirtualHost { get; init; }
    public string Host { get; init; }
    public string UserName { get; init; }
    public string Password { get; init; }
}