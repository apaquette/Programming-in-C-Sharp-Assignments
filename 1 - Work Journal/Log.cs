public class Log{
    public Log(string? completed, string? nextAction, DateTime logTime){
        Completed = completed;
        NextAction = nextAction;
        LogTime = logTime;
    }

    public string? Completed { get; set; }
    public string? NextAction { get; set; }
    public DateTime LogTime { get; set; }

    public override string ToString() => $"\n\n-----------------------------------\n\nTimestamp: {LogTime}\n\nCompleted: {Completed}\n\nNext Action: {NextAction}\n\n-----------------------------------\n\n";
}