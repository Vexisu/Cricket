using Cricket.Interpreter;

namespace CricketBootstrap.Environment; 

public class EnvironmentManager {
    public Dictionary<string, Interpreter> EnvironmentInterpreter { get; }

    public EnvironmentManager() {
        EnvironmentInterpreter = new Dictionary<string, Interpreter>();
    }
    
}