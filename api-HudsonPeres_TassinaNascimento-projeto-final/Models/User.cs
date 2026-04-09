namespace api_HudsonPeres_TassinaNascimento_projeto_final.Models;

public class User
{
    public int ID {get; set; }
    public string Username {get; set; } = string.Empty;
    public string PasswordHash {get; set; } = string.Empty;
    public string Email {get; set; } = string.Empty;
}
