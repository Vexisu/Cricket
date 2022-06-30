using System;
using System.IO;

namespace Cricket;

internal static class Program {
    private static bool _debug;

/**
 * Funkcja main interpretera
 * <param name="args">Argumenty przyjmowane przy uruchomieniuW</param>
 */

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

/**
 * Funkcja zwracająca informację w przypadku braku podania pliku źródłowego.
 */
    private static void NoPathPresent() {
        Console.Out.WriteLine("No path to source file.");
        Environment.Exit(101);
    }
}