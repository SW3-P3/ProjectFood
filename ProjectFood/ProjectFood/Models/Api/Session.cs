namespace eTilbudsharp.Api
{
    public class Session
    {
        public string Token { get; set; }
        public string Expires { get; set; }
        public string User { get; set; }
        public string Provider { get; set; }
        public Permissions Permissions { get; set; }
    }
}
