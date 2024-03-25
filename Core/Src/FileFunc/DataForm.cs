namespace Core.FileFunc;
public class DataForm {
    public string Name { get; set; }
    public string Content { get; set; }
    public bool EnableCrypt { get; set; }

    public DataForm() {
        Name = string.Empty;
        Content = string.Empty;
        EnableCrypt = false;
    }
    public DataForm(string name, string content, bool enableCrypt) {
        Name = name;
        Content = content;
        EnableCrypt = enableCrypt;
    }
}