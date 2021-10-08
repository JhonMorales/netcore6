namespace WebApplication2.DTOs
{
    public class AuthenticateResponseDTO
    {
        public string Token { get; set; }
        public DateTime ExpirateAt { get; set; }
    }
}
