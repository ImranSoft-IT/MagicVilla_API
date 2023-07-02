namespace MagicVilla_VillaAPI.Models.Dto
{
    public class LoginResponseDTO
    {
        //public LocalUser User { get; set; }
        public UserDTO User { get; set; } /*For Identity*/
        //public string Role { get; set; }
        public string Token { get; set; }
    }
}
