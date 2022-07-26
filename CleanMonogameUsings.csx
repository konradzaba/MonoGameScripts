/*
This script cleans unnecessary usings in MonoGame 3.8.1 solution when performing a migration. 
It cleans all Microsoft.Xna.Framework.* usings which are no longer required implicitely due to global using feature available in C# 10.
Usage:
1) launch Developer Command Prompt (standard CMD won't work)
2) type in "csi CleanMonogameUsings.csx c:\path\to\your\solution\folder" 
3) run the script
*/


using System.Text.RegularExpressions;
using System.Linq;

var parameters = Environment.GetCommandLineArgs();
if (Args.Count() != 1)
{
    throw new ArgumentException($"Use one parameter - path to folder with solution. Used {Args.Count()} parameters");
}
var files = Directory.GetFiles(Args[0], "*.cs", SearchOption.AllDirectories);
var usingToClean = "using Microsoft.Xna.Framework";
Regex regex = new Regex($@"^{Regex.Escape(usingToClean)}\b");
var usingsList = new List<string>();
foreach (string file in files)
{
    var fileContent = File.ReadLines(file);
    if (fileContent.Any(line => regex.IsMatch(line)))
    {
        //determine the encoding
        Encoding encoding;
        using (var reader = new StreamReader(file))
        {
            encoding = reader.CurrentEncoding;
        }

        //write the using into the list
        fileContent.Where(line=> regex.IsMatch(line))
            .ToList()
            .ForEach(line=> usingsList.Add(line));

        //rewrite file
        File.WriteAllLines(file, fileContent
           .Where(line => !regex.IsMatch(line))
           .ToArray(), encoding);
    }
}

//Generate global usings
usingsList = usingsList
    .Distinct()
    .Select(line => line.Replace("using",string.Empty))
    .Select(line => line.Trim())
    .Select(line => $"<Using Include=\"{line.Replace(";",string.Empty)}\" />")
    .ToList();
usingsList.Insert(0, "<ItemGroup>");
usingsList.Add("</ItemGroup>");
usingsList.Add("</Project>");
     
//append global usings to project files
var projectFiles = Directory.GetFiles(Args[0], "*.csproj", SearchOption.AllDirectories);
foreach (string file in projectFiles)
{
    File.WriteAllText(file, File.ReadAllText(file).Replace("</Project>", string.Empty));
    File.AppendAllLines(file, usingsList);
}
