using System.ComponentModel;

namespace GoogleDynamicDNS
{
    public class Config
    {
        [DefaultValue(300000)]
        public int UpdateFrequency { get; set; }
        public List<DynamicDomain>? Hosts { get; set; }
    }

    public class DynamicDomain
    {
        public string Hostname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
