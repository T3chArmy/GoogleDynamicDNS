using System.Text.Json;

List<DynamicConfig>? dynDNSConfig = JsonSerializer.Deserialize<List<DynamicConfig>>(@File.OpenText("dyndns.json").ReadToEnd());

Timer updateEvent = new(new TimerCallback(UpdateDnyDNS), new {}, 0, 300000);
Console.ReadLine();
updateEvent.Dispose();

async void UpdateDnyDNS(object? sender)
{
    if (dynDNSConfig == null) return;
    ColorConsole.WriteWrappedHeader("Update Event Started - " + DateTime.Now.ToString());
    foreach(DynamicConfig config in dynDNSConfig)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.ASCIIEncoding.UTF8.GetBytes(config.Username + ":" + config.Password)));
        client.Timeout = TimeSpan.FromSeconds(5);
        var response = await client.GetStringAsync("https://domains.google.com/nic/update?hostname=" + config.Hostname);
        string responseColor = response.Split(" ")[0] switch
        {
            "good" => "green",
            "nochg" => "yellow",
            _ => "red"
        };
        ColorConsole.WriteEmbeddedColorLine($"Hostname: [blue]{config.Hostname}[/blue]; Response: [{responseColor}]{response}[/{responseColor}];");
        client.Dispose();
    }
    ColorConsole.WriteWrappedHeader("Update Event Completed - " + DateTime.Now.ToString());
}

public struct DynamicConfig
{
    public string Hostname { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}