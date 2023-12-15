using CommandLine;

namespace DC.PoC.DaLayers.Console.Options;

[Verb("upload", HelpText = "Upload a file to the Joint Tariff Web API")]
public class UploadOptions
{
    [Value(0, Required = true, MetaName = "InputPath", HelpText = "The File to be uploaded.")]
    public string Filename { get; set; } = "DoesNotExist.xlsx";
}