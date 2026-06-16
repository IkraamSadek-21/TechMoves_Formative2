using System.ComponentModel.DataAnnotations;

namespace TechMoves_Formative2.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public string Expiry { get; set; }
    }
}