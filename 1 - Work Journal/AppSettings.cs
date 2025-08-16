using static System.Console;
using static System.IO.Directory;
using static System.IO.Path;
using static System.Environment;
using Newtonsoft.Json;

public static class AppSettings{
    private static string _settingsDirectory = Combine(GetFolderPath(SpecialFolder.LocalApplicationData), "Work Journal", "Save Data");
    private static string _settingsFileDirectory = Combine(_settingsDirectory, "AppSettings.json");
    private static string? _userDataFileDirectory;
    private static string? _logListDir;

    public static string DateFormat { get => "yyyy-MM-dd"; }

    public static void Initialize(){
        //create directory if it doesn't already exist
        if(!Exists(_settingsDirectory)){ CreateDirectory(_settingsDirectory); }
        //create file if it doesn't already exist
        if(!File.Exists(_settingsFileDirectory)){
            StreamWriter sw = File.CreateText(_settingsFileDirectory);
            sw.Close();
        }
        //assign user save data
        StreamReader sr = File.OpenText(_settingsFileDirectory);
        string jsonData = sr.ReadToEnd();
        string[] userDataDirList = JsonConvert.DeserializeObject<string[]>(jsonData) ?? new string[0];
        sr.Close();
        string userDataDir = "";
        foreach (string s in userDataDirList){ userDataDir = Combine(userDataDir, s); }
        
        if(Exists(userDataDir)){//user assigned directory
            _userDataFileDirectory = userDataDir;
        }else{//default directory
            _userDataFileDirectory = Combine(GetFolderPath(SpecialFolder.Personal), "Work Journal", "Save Data");//default directory
        }
        
        _logListDir = Combine(_userDataFileDirectory!, "Logs");
        
        if(!Exists(_logListDir)){ CreateDirectory(_logListDir); }
    }

    public static void SetUserDirectory(){
        Write("Enter directory path for data storage: ");
        string? dir = ReadLine();
        if(Exists(dir)){
            _userDataFileDirectory = Combine(dir, "Save Data");
            CreateDirectory(_userDataFileDirectory);
            StreamWriter sw = File.CreateText(_settingsFileDirectory);
            string[] split = _userDataFileDirectory.Split('\\');//split each folder
            sw.WriteLine(JsonConvert.SerializeObject(split));//write as string array to separate each folder
            sw.Close();
        }else{
            WriteLine("Invalid Directory");
        }
    }
    
    public static void Display<T>(this List<T> list){ foreach (T t in list){ WriteLine(t); }}

    public static List<T> Load<T>(string file) {
        List<T> list = new List<T>();
        string dir = GetDirectory(list.GetType().GetGenericArguments().Single().Name, file);
        if(File.Exists(dir)){
            StreamReader sr = File.OpenText(dir);
            string jsonData = sr.ReadToEnd();
            list = JsonConvert.DeserializeObject<List<T>>(jsonData)!;
            sr.Close();
        }
        
        return list;
    }

    public static void Save<T>(this T item, string file){
        string dir = GetDirectory(item?.GetType().Name ?? "", file);
        
        List<T> list = new List<T>();
        if(File.Exists(dir)){ list = Load<T>(dir); }
        list.Add(item);
        
        StreamWriter sw = File.CreateText(dir);
        sw.WriteLine(JsonConvert.SerializeObject(list));
        sw.Close();
    }

    public static void Save<T>(this List<T> list, string file){
        string dir = GetDirectory(list.GetType().GetGenericArguments().Single().Name, file);

        StreamWriter sw = File.CreateText(dir);
        sw.WriteLine(JsonConvert.SerializeObject(list));
        sw.Close();
    }

    private static string GetDirectory(string className, string file){
        switch(className){
            case "Log":
                return Combine(_logListDir ?? "", file);
            case "Task":
                return Combine(_userDataFileDirectory ?? "", file);
            case "Flashcard":
                return Combine(_userDataFileDirectory ?? "", file);
        }
        
        return "";
    }
}