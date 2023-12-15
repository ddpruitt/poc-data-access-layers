namespace DC.PoC.DaLayers.Console.Options;

public class InputFileProcessOptions
{
    public static string ConfigurationSectionName = "InputFileProcess";
    private string _archiveDirectory = string.Empty;
    private string _errorDirectory = string.Empty;

    public string ArchiveDirectory
    {
        get => _archiveDirectory;
        set
        {
            _archiveDirectory = value;
            ArchiveDirectoryInfo = new DirectoryInfo(value);
        }
    }

    public string ErrorDirectory
    {
        get => _errorDirectory;
        set
        {
            _errorDirectory = value;
            ErrorDirectoryInfo = new DirectoryInfo(value);
        }
    }


    public DirectoryInfo? ArchiveDirectoryInfo { get; private set; }

    public DirectoryInfo? ErrorDirectoryInfo { get; private set; }
}