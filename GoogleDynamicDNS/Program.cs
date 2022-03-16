using System.Text.Json;

namespace GoogleDynamicDNS
{

    public class Program {
        static Config? dynDNSConfig;
        static Timer? updateEvent;
        public static void Main()
        {
            dynDNSConfig = JsonSerializer.Deserialize<Config>(File.ReadAllText("dyndns.json"));

            if (dynDNSConfig == null) return;
            updateEvent = new(new TimerCallback(UpdateDnyDNS), new { }, 0, dynDNSConfig.UpdateFrequency);
            Console.ReadLine();
            updateEvent.Dispose();
        }

        private static async void UpdateDnyDNS(object? sender)
        {
            if (dynDNSConfig?.Hosts == null) return;
            ColorConsole.WriteWrappedHeader("Update Event Started - " + DateTime.Now.ToString());
            foreach (DynamicDomain domain in dynDNSConfig.Hosts)
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.ASCIIEncoding.UTF8.GetBytes(domain.Username + ":" + domain.Password)));
                client.Timeout = TimeSpan.FromSeconds(5);
                var response = await client.GetStringAsync("https://domains.google.com/nic/update?hostname=" + domain.Hostname);
                string responseColor = response.Split(" ")[0] switch
                {
                    "good" => "green",
                    "nochg" => "yellow",
                    _ => "red"
                };
                ColorConsole.WriteEmbeddedColorLine($"Hostname: [blue]{domain.Hostname}[/blue]; Response: [{responseColor}]{response}[/{responseColor}];");
                client.Dispose();
            }
            ColorConsole.WriteWrappedHeader("Update Event Completed - " + DateTime.Now.ToString());
        }
    }
}