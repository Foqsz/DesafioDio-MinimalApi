namespace DesafioDio_MinimalApi.Project.Domain.DTOs
{
    public record AdmConnectedDTO
    {
        public string? Email { get; set; }
        public string? Perfil { get; set; }    
        public string Token { get; set; }
    }
}
