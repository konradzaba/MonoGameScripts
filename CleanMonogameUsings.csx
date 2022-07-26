/*
This script cleans unnecessary usings in MonoGame 3.8.1 solution when performing a migration. 
It cleans all Microsoft.Xna.Framework.* usings which are no longer required implicitely due to global using feature available in C# 10.
Usage:
1) launch Developer command prompt (standard CMD won't work)
2) type in "csi CleanMonogameUsings.csx c:\path\to\your\solution\folder"
*/


using System.Text.RegularExpressions;

var parameters = Environment.GetCommandLineArgs();
if (Args.Count() != 1)
{
    throw new ArgumentException($"Use single parameter - path to folder with solution. Used {Args.Count()} parameters");
}
string[] files = Directory.GetFiles(Args[0], "*.cs",SearchOption.AllDirectories);
foreach (string file in files)
{
    var usingToClean = "using Microsoft.Xna.Framework";
    Regex regex = new Regex($@"^{Regex.Escape(usingToClean)}\b");

    File.WriteAllLines(file, File
      .ReadLines(file)
      .Where(line => !regex.IsMatch(line))
      .ToArray());
}

