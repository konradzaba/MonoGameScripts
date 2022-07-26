using System.Text.RegularExpressions;

//var path = @"C:\github\VornWinter";
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

