namespace WebAppForJWT.DTO
{
    public class JwtSettings //см appsettings.json
    {
        public string Key { get; set; } //так явно лучше не называть
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int TokenLifeTime { get; set; }
    }
}
