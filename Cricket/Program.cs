using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace Cricket;

internal static class Program {
    public static bool Debug;

    private static void Main(string[] args) {
        HackDecimals();
        if (args.Length > 0) {
            if (args[0] == "--debug") {
                Debug = true;
                if (args.Length <= 1) NoPathPresent();
            }
            var path = args[Debug ? 1 : 0];
            if (!File.Exists(path)) {
                Console.Out.WriteLine("File with given path does not exists.");
                Environment.Exit(102);
            }
            var interpreter = new Interpreter.Interpreter(path);
            interpreter.StartInterpreter();
        }
        else {
            NoPathPresent();
        }
    }

    private static void NoPathPresent() {
        Console.Out.WriteLine("No path to source file.");
        Environment.Exit(101);
    }

    // This dirty hack fixes decimal separator in CSharp
    private static void HackDecimals() {
        var cultureInfo = new CultureInfo("en-US");
        cultureInfo.NumberFormat = NumberFormatInfo.InvariantInfo;
        Thread.CurrentThread.CurrentCulture = cultureInfo;
    }
}