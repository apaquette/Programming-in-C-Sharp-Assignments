namespace MyGames.Models;

public class Game{
    public int GameId { get; set; }
    public string? Title { get; set; }
    public DateTime ReleaseDate { get; set; }
    public float HoursPlayed { get; set; }
    public bool Favorite {get; set; }
    public int PublisherId { get; set; }
    public Publisher? Publisher { get; set; }
}