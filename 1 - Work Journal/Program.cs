using static AppSettings;
using static System.Console;

Initialize();//initialize AppSettings

string command = "";
string[] flags = new string[0];

if(args.Count() != 0){
    command = args.First();  //get command
    flags = args.Skip(1).ToArray();    //separate flags
}

switch(command){
    case "setup":
        if(flags.Contains("--dir") || flags.Contains("-d"))
            SetUserDirectory();//call set user directory of AppSettings
        break;
    case "log":
        String flag = flags.Count() != 0 ? flags.First() : "";
        switch (flag){
            case "list":
                List();
                break;
            default:
                Write("Completed: ");
                string? completed = ReadLine();
                Write("Next Action: ");
                string? nextAction = ReadLine();
                Log log = new Log(completed, nextAction, DateTime.Now);
                log.Save($"{DateTime.Now.ToString(DateFormat)}.json");
                List<Log> last = new List<Log>();
                last.Add(log);
                last.Save("Last.json");
                break;
        }
        break;
    case "task":
        Task();
        break;
    case "card":
        Card();
        break;
}

void List(){
    string date = "";
    string[] flagsLocal = flags.Skip(1).Count() == 0 ? new String[]{""}:flags.Skip(1).ToArray();
    switch(flagsLocal.First()){
        case "--date":
        case "-d":
            date = flagsLocal[1];
            break;
        case "--today":
        case "-t":
            date = DateTime.Today.ToString("yyyy-MM-dd");
            break;
        case "--yesterday":
        case "-y":
            date = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
            break;
        case "--last":
        case "-l":
            date = "Last";
            break;
        default:
            WriteLine("wj log list flags:");
            WriteLine("-d|--date\tShow logs of specific date. Provide date with format 'yyyy-MM-dd'");
            WriteLine("-t|--today\tShow logs for today");
            WriteLine("-y|--yesterday\tShow logs for yesterday");
            WriteLine("-l|--last\tLast entry");
            break;
    }
    List<Log> logs = Load<Log>(date+".json");
    if(logs.Count() > 0){
        Load<Log>(date+".json").Display();
    }else{
        WriteLine("No logs to display");
    }
    
}

void Task(){
    string flag = flags.Count() == 0 ? "":flags.First();
    switch(flag){
        case "list":
            ListTask();
            break;
        case "complete":
            CompleteTask();
            break;
        default:
            AddTask();
            break;
    }
    void AddTask(){
        int descIndex = Array.IndexOf(flags, "--desc");
        string desc = "";
        if(descIndex != -1) {
            desc = flags[descIndex + 1];
        }else{
            Write("Description: ");
            desc = ReadLine()!;
        }
        
        int dateIndex = Array.IndexOf(flags, ("--date"));
        string dateString = "";
        if(dateIndex != -1) {
            dateString = flags[dateIndex + 1];
        }
        DateTime date = new DateTime();
        bool invalid = true;
        do{
            try{
                if(dateString == "") {
                    Write("Due Date [yyyy-MM-dd]: ");
                    dateString = ReadLine()!;
                }
                date = DateTime.ParseExact(dateString, "yyyy-MM-dd", null);
                invalid = false;
            }catch{
                WriteLine("Invalid Date Format");
                invalid = true;
                dateString = "";
            }
        }while(invalid);
        new Task(desc, date).Save("tasks.json");
    }
    void CompleteTask(){
        List<Task> taskList = Load<Task>("tasks.json");
        List<Task> incompleteList = GetTasksBasedOn(Load<Task>("tasks.json"), Incomplete);
        if(incompleteList.Count() == 0){
            WriteLine("No incomplete tasks");
        }else{
            List<string> ids = new List<string>();
            if(flags.Count() == 3 && (flags[1] == "--id" || flags[1] == "-i")){
                ids = flags[2].Split(",").ToList<string>();
            }else{//display numbered list for incomplete
                string menu = "Select from the following to mark as complete\n\n";
                int id = 1, uInput = 0, exit = 0;
                bool isValid = false;

                foreach(Task t in taskList){
                    if(!t.Complete){
                        menu += $"{id++} - {t.ID.ToString()}\n{t.Description}\n\n";
                        incompleteList.Add(t);
                    }
                }
                menu += $"{id} - Cancel\n\nEnter: ";
                exit = id;
                do{
                    try{
                        Write(menu);
                        isValid = int.TryParse(ReadLine(), out uInput);
                    }catch{
                        isValid = false;
                    }

                }while(!isValid);
                if(uInput == exit){
                    incompleteList.Clear();
                }else{
                    ids.Add(incompleteList[uInput - 1].ID.ToString());
                }
            }
            
            //mark complete
            foreach(Task t in incompleteList){
                if(ids.Contains(t.ID.ToString())){
                    t.Complete = true;
                    t.CompleteDate = DateTime.Now;
                }
            }

            //put back in main list
            for(int i = 0; i < taskList.Count(); i++){
                foreach(Task t in incompleteList){
                    if(taskList[i].ID.ToString().Equals(t.ID.ToString())){
                        taskList[i] = t;
                    }
                }
            }

            taskList.Save("tasks.json"); //overwrite .json file to save completed list
        }
    }
    void ListTask(){
        List<Task> taskList = Load<Task>("tasks.json");
        List<Task> displayList = new List<Task>();

        string flag = flags.Count() > 1 ? flags[1]:"";

        switch(flag){
            case "--all":
            case "-a":
                displayList = taskList;
                break;
            case "--up-coming":
            case "-u":
                displayList = GetTasksBasedOn(taskList, Upcoming);
                break;
            case "--today":
            case "-t":
                displayList = GetTasksBasedOn(taskList, Today);
                break;
            case "--overdue":
            case "-o":
                displayList = GetTasksBasedOn(taskList, Overdue);
                break;
            case "--completed":
            case "-c":
                displayList = GetTasksBasedOn(taskList, Complete);
                break;
            default:
                displayList = GetTasksBasedOn(taskList, Incomplete);
                break;
        }

        if(displayList.Count() == 0){
            WriteLine("No task(s) matches criteria");
        }else{
            displayList.Display();
        }
    }
}

void Card(){
    string flag = flags.Count() == 0 ? "":flags.First();
    
    if(flag == "quiz"){
        List<Flashcard> deck = Load<Flashcard>("flashcards.json");
        if(deck.Count() == 0){
            WriteLine("No cards available");
        }else{//play flashcards
            
            List<Flashcard> randomDeck = new List<Flashcard>();
            int deckCount = 1;

            if (flags[1] == "--number" || flags[1] == "-n"){
                deckCount = Int32.Parse(flags[2]) > deck.Count() ? deckCount:Int32.Parse(flags[2]);
            }
            
            for(int i = 1; i <= deckCount; i++){
                bool contains = true;
                var rand = new Random();
                while(contains){
                    int randNum = rand.Next(0, deck.Count());
                    Flashcard card = deck[randNum];
                    if(!randomDeck.Contains(card)){
                        randomDeck.Add(card);
                        contains = false;
                    }
                }
            }
            foreach (Flashcard card in randomDeck){
                WriteLine(card);
                WriteLine("Press Enter to show the answer");
                ReadLine();
                WriteLine($"Answer: {card.Answer}\n");

                bool invalid = true;
                while(invalid){
                    Write("[E]asily Remembered\n[H]ard to remember\n[D]idn't Remember\n\nEnter: ");
                    string? input = ReadLine();
                    switch(input?.ToLower() ?? ""){
                        case "e":
                            invalid = false;
                            deck.Remove(card);
                            card.Save("archivedCards.json");
                            deck.Save("flashcards.json");
                            break;
                        case "h":
                        case "d":
                            invalid = false;
                            break;
                        default:
                            invalid = true;
                            WriteLine("\nInvalid Input\n");
                            break;
                    }
                }
            }
        }
    }else{//create new flashcard
        int questionIndex = Array.IndexOf(flags, "--question");
        string question = "";
        if(questionIndex != -1) {
            question = flags[questionIndex + 1];
        }else{
            Write("Question: ");
            question = ReadLine()!;
        }

        int answerIndex = Array.IndexOf(flags, "--answer");
        string answer = "";
        if(answerIndex != -1) {
            answer = flags[answerIndex + 1];
        }else{
            Write("Answer: ");
            answer = ReadLine()!;
        }

        new Flashcard(question, answer).Save("flashcards.json");
    }
}

List<Task> GetTasksBasedOn(List<Task> list, TaskCriteria criteria){
    List<Task> displayList = new List<Task>();
    foreach(Task t in list){
        if(criteria(t)){
            displayList.Add(t);
        }
    }
    return displayList;
}

bool Today(Task t) => t.Date == DateTime.Today;
bool Complete(Task t) => t.Complete;
bool Incomplete(Task t) => !t.Complete;
bool Overdue(Task t) => t.Status().Equals("Overdue");
bool Upcoming(Task t) => t.Date > DateTime.Now && t.Date < DateTime.Now.AddDays(7);

delegate bool TaskCriteria(Task t);