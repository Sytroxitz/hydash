
using System.Diagnostics;
using System.Configuration;
using System;
using System.IO;
using System.Xml.Linq;
using hydash.Client;
using System.Reflection;

/*
[assembly: AssemblyVersion("1.0")]
[assembly: AssemblyInformationalVersion("1.0")]
[assembly: AssemblyFileVersion("1.0")]
[assembly: AssemblyCompany("hydash")]
[assembly: AssemblyProduct("hydash.Client")]
[assembly: AssemblyTitle("hydash.Client")]
*/

namespace hydash.Client;
class Program
{
	static void Main(string[] args)
	{
		Assembly assembly = typeof(Program).Assembly;
		AssemblyName name = assembly.GetName();
		Version version = name.Version;

		Console.Title = "HYDASH - Client";

		string filePath = "hydash.config";
		/*

			██╗  ██╗██╗   ██╗██████╗  █████╗ ███████╗██╗  ██╗
			██║  ██║╚██╗ ██╔╝██╔══██╗██╔══██╗██╔════╝██║  ██║
			███████║ ╚████╔╝ ██║  ██║███████║███████╗███████║
			██╔══██║  ╚██╔╝  ██║  ██║██╔══██║╚════██║██╔══██║
			██║  ██║   ██║   ██████╔╝██║  ██║███████║██║  ██║
			╚═╝  ╚═╝   ╚═╝   ╚═════╝ ╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝
			Version 1.0.0-dev developed by https://slxy.dev/

		*/
		Console.WriteLine("");
		Console.WriteLine("\t\u2588\u2588\u2557  \u2588\u2588\u2557\u2588\u2588\u2557   \u2588\u2588\u2557\u2588\u2588\u2588\u2588\u2588\u2588\u2557  \u2588\u2588\u2588\u2588\u2588\u2557 \u2588\u2588\u2588\u2588\u2588\u2588\u2588\u2557\u2588\u2588\u2557  \u2588\u2588\u2557");
		Console.WriteLine("\t\u2588\u2588\u2551  \u2588\u2588\u2551\u255a\u2588\u2588\u2557 \u2588\u2588\u2554\u255d\u2588\u2588\u2554\u2550\u2550\u2588\u2588\u2557\u2588\u2588\u2554\u2550\u2550\u2588\u2588\u2557\u2588\u2588\u2554\u2550\u2550\u2550\u2550\u255d\u2588\u2588\u2551  \u2588\u2588\u2551");
		Console.WriteLine("\t\u2588\u2588\u2588\u2588\u2588\u2588\u2588\u2551 \u255a\u2588\u2588\u2588\u2588\u2554\u255d \u2588\u2588\u2551  \u2588\u2588\u2551\u2588\u2588\u2588\u2588\u2588\u2588\u2588\u2551\u2588\u2588\u2588\u2588\u2588\u2588\u2588\u2557\u2588\u2588\u2588\u2588\u2588\u2588\u2588\u2551");
		Console.WriteLine("\t\u2588\u2588\u2554\u2550\u2550\u2588\u2588\u2551  \u255a\u2588\u2588\u2554\u255d  \u2588\u2588\u2551  \u2588\u2588\u2551\u2588\u2588\u2554\u2550\u2550\u2588\u2588\u2551\u255a\u2550\u2550\u2550\u2550\u2588\u2588\u2551\u2588\u2588\u2554\u2550\u2550\u2588\u2588\u2551");
		Console.WriteLine("\t\u2588\u2588\u2551  \u2588\u2588\u2551   \u2588\u2588\u2551   \u2588\u2588\u2588\u2588\u2588\u2588\u2554\u255d\u2588\u2588\u2551  \u2588\u2588\u2551\u2588\u2588\u2588\u2588\u2588\u2588\u2588\u2551\u2588\u2588\u2551  \u2588\u2588\u2551");
		Console.WriteLine("\t\u255a\u2550\u255d  \u255a\u2550\u255d   \u255a\u2550\u255d   \u255a\u2550\u2550\u2550\u2550\u2550\u255d \u255a\u2550\u255d  \u255a\u2550\u255d\u255a\u2550\u2550\u2550\u2550\u2550\u2550\u255d\u255a\u2550\u255d  \u255a\u2550\u255d");
		Console.ForegroundColor = ConsoleColor.DarkGray;
		Console.WriteLine("\tVersion {0}-client developed by https://slxy.dev/", version);
		Console.ResetColor();
		Console.WriteLine("");

		websocket.Websocket.Connect(args);

		WritePrefixedLine(PrefixType.Input, "Please provide your API Token from (https://hydash.net/account/token/): ", true);
		string token = Console.ReadLine();
		WritePrefixedLine(PrefixType.Input, "Token accepted!", false, ConsoleColor.DarkGreen);
		WritePrefixedLine(PrefixType.Input, "What's your Server start file? (Example: start.bat): ", true);
		string serverStartFile = Console.ReadLine();
		WritePrefixedLine(PrefixType.Input, "File found!", false, ConsoleColor.DarkGreen);

		if (!File.Exists(filePath))
		{
			// Create a custom .config file
			var config = new XDocument(
				new XDeclaration("1.0", "utf-8", null),
				new XElement("configuration",
					new XElement("appSettings",
						new XElement("add", new XAttribute("key", "cmd"), new XAttribute("value", "cmd.exe")),
						new XElement("add", new XAttribute("key", "arg"), new XAttribute("value", "/c echo Hello World"))
					)
				)
			);

			config.Save(filePath);
			WritePrefixedLine(PrefixType.Info, $"The '{filePath}' file has been created.");
		}
		else
		{
			WritePrefixedLine(PrefixType.Info, $"The '{filePath}' already exists.");
		}

		// Load and parse the custom .config file
		var configDoc = XDocument.Load(filePath);
		var appSettings = configDoc.Descendants("add")
			.ToDictionary(
				el => el.Attribute("key").Value,
				el => el.Attribute("value").Value);

		// Use the settings to start a process
		if (appSettings.TryGetValue("cmd", out string cmd) && appSettings.TryGetValue("args", out string arg))
		{
			var proc = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = cmd,
					Arguments = arg,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					CreateNoWindow = false
				}
			};

			proc.Start();
			var output = proc.StandardOutput.ReadToEnd();
			proc.WaitForExit();
			Console.WriteLine(output);
		}
		else
		{
			WritePrefixedLine(PrefixType.Error, "Command or arguments not found in the configuration file.");
		}
	}

	public static void WritePrefixedLine(PrefixType prefixType, string message, bool inLine = false, ConsoleColor messageColor = ConsoleColor.White)
	{
		switch (prefixType)
		{
			case PrefixType.Info:
				Console.ForegroundColor = ConsoleColor.Green;
				break;
			case PrefixType.Warning:
				Console.ForegroundColor = ConsoleColor.Yellow;
				break;
			case PrefixType.Error:
				Console.ForegroundColor = ConsoleColor.Red;
				break;
			case PrefixType.Debug:
				Console.ForegroundColor = ConsoleColor.Cyan;
				break;
			case PrefixType.Input:
				Console.ForegroundColor = ConsoleColor.DarkMagenta;
				break;
			default:
				Console.ForegroundColor = ConsoleColor.White;
				break;
		}
		Console.Write($"[{prefixType}] ");
		Console.ForegroundColor = messageColor;
		if (inLine)
		{
			Console.Write(message);
		}
		else
		{
			Console.WriteLine(message);
		}
		Console.ResetColor();
	}
}