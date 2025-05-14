namespace Jilo.Dto
{
    public class AddGameToUserRequest
    {
        public int GameId { get; set; }
        public string? Rank { get; set; }
        public string? Role { get; set; }
        public string? TimeInGame { get; set; }
    }
}
