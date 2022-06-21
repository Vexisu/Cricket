using Cricket.Interpreter;
using CricketBootstrap.Environment;
using CricketBootstrap.Lib;
using CricketBootstrap.Server;

namespace CricketBootstrap;

public static class CricketBootstrap {
    public static void Main() {
        Interpreter.Debug = true;
        var environmentManager = LoadEnvironments();
        var dynamicDocuments = LoadDocuments(environmentManager);
        try {
            var httpServer = new HttpServer(dynamicDocuments);
        }
        catch (Exception e) {
            Interpreter.HandleException(e);
            throw;
        }
    }

    private static EnvironmentManager LoadEnvironments() {
        var environmentManager = new EnvironmentManager();
        var config = File.ReadAllLines("config/environments.config");
        foreach (var configLine in config) {
            var arguments = configLine.Split(' ');
            if (arguments.Length != 2) {
                Console.Out.WriteLine("Error in environments.config.");
                System.Environment.Exit(201);
            }
            var interpreter = new Interpreter("www/" + arguments[1]);
            CastStringToInteger.Link(interpreter);
            CastStringToFloat.Link(interpreter);
            HttpRequestValue.Link(interpreter);
            interpreter.StartInterpreter();
            environmentManager.EnvironmentsInterpreter[arguments[0]] = interpreter;
        }
        return environmentManager;
    }

    private static Dictionary<string, DynamicDocument> LoadDocuments(EnvironmentManager environmentManager) {
        var dynamicDocuments = new Dictionary<string, DynamicDocument>();
        var config = File.ReadAllLines("config/documents.config");
        foreach (var configLine in config) {
            var arguments = configLine.Split(" ");
            if (arguments.Length != 2) {
                Console.Out.WriteLine("Error in documents.config.");
                System.Environment.Exit(202);
            }
            var staticDocument = File.ReadAllText("www/" + arguments[1]);
            var dynamicDocument = new DynamicDocument(staticDocument, environmentManager);
            dynamicDocuments[arguments[0]] = dynamicDocument;
        }
        return dynamicDocuments;
    }
}