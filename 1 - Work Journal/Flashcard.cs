public class Flashcard{
    public Flashcard(string question, string answer){
        Question = question;
        Answer = answer;
    }

    public string? Question { get; set; }
    public string? Answer { get; set; }

    public override string ToString() => Question ?? "";
}