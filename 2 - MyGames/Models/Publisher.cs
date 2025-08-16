namespace MyGames.Models;

public class Publisher{
    public int PublisherId { get; set; }
    public string? Name { get; set; }
    public string? Country { get; set; }
    public int FoundingYear {get; set; }
    public List<Game>? Games { get; set; }
}