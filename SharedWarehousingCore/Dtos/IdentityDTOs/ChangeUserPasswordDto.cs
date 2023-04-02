namespace SharedWarehousingCore.Dtos.IdentityDTOs;

public class ChangeUserPasswordDto
{
    public string UserEmail { get; set; }
    public string Token { get; set; }
    public string NewPassword { get; set; }
}