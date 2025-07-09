namespace AwlrAziz.Options
{
    public class MqttClientSettings
    {
        public string Address { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Port { get; set; }
    }
}