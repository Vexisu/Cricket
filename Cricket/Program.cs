using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace Cricket;

internal static class Program {
    private static bool _debug;

    private static void Main(string[] args) {
        if (args.Length > 0) {
            if (args[0] == "--debug") {
                _debug = true;
                if (args.Length <= 1) NoPathPresent();
            }
            var path = args[_debug ? 1 : 0];
            if (!File.Exists(path)) {
                Console.Out.WriteLine("File with given path does not exists.");
                Environment.Exit(102);
            }
            var interpreter = new Interpreter.Interpreter(path);
            Interpreter.Interpreter.Debug = _debug;
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
}