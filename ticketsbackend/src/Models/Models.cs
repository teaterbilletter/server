using System.ComponentModel.DataAnnotations;

namespace ticketsbackend.Models
{
    public class Login
    {
        [Required] public string Name { get; set; }
        [Required] public string Password { get; set; }
    }

    public class LoginResult
    {
        [Required] public string Response;
        [Required] public string Token;

        public LoginResult(string response, string token)
        {
            Token = token;
            Response = response;
        }
    }
}