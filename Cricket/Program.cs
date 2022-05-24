using System;
using System.IO;

namespace Cricket;

internal class Program {
    private static void Main(string[] args) {
        if (args.Length > 0) {
            var path = args[0];
            if (!File.Exists(path)) {
                Console.Out.WriteLine("File with given path does not exists.");
                Environment.Exit(102);
            }
            var interpreter = new Interpreter.Interpreter(path);
            interpreter.StartInterpreter();
        }
        else {
            Console.Out.WriteLine("No path to source file.");
            Environment.Exit(101);
        }
    }
}