namespace Pertuk.Business.Options
{
    public class JwtOption
    {
        public string Secret { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
    }
}
