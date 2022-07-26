/*
This script cleans unnecessary usings in MonoGame 3.8.1 solution when performing a migration. 
It cleans all Microsoft.Xna.Framework.* usings which are no longer required implicitely due to global using feature available in C# 10.
Usage:
1) launch Developer Command Prompt (standard CMD won't work)
2) type in "csi CleanMonogameUsings.csx c:\path\to\your\solution\folder c:\path\to\your\solution\GlobalUsings.cs" 
3) add the newly generated file to all projects (suggestion: add as link to Properties)
*/


using System.Text.RegularExpressions;
using System.Linq;

var parameters = Environment.GetCommandLineArgs();
if (Args.Count() != 2)
{
    throw new ArgumentException($"Use two parameters - path to folder with solution & path to generated file with global usings. Used {Args.Count()} parameters");
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

//Generate global using file
usingsList = usingsList
    .Distinct()
    .Select(line => $"global {line}")
    .ToList();

File.WriteAllLines(Args[1], usingsList);
