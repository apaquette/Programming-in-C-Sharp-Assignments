public class Task{
    public Task(string? description, DateTime? date){
        Description = description;
        Date = date;
        Complete = false;
    }

    public string? Description { get; set; }
    public DateTime? Date { get; set; }
    public bool Complete { get; set; }
    public DateTime? CompleteDate { get; set; } = null;

    public Guid ID { get; set; } = Guid.NewGuid();

    public string Status(){
        if(Complete)
            return "Complete";
        if(Date < DateTime.Now)
            return "Overdue";
        return "Incomplete";
    }

    public override string ToString() => $"-----------------------------------\n\nID: {ID}\n\nDescription: {Description}\n\nDue Date: {Date}\n\nDate Completed: {CompleteDate}\n\nStatus: {Status()}\n\n-----------------------------------";
}