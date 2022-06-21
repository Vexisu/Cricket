using System.Collections.Specialized;
using Cricket.Interpreter;
using Cricket.Interpreter.Parser.Statement.Expression;

namespace CricketBootstrap.Environment;

public class EnvironmentManager {
    public Dictionary<string, Interpreter> EnvironmentsInterpreter { get; }

    public EnvironmentManager() {
        EnvironmentsInterpreter = new Dictionary<string, Interpreter>();
    }

    public void AddGetRequestValues(NameValueCollection nameValueCollection) {
        foreach (var key in nameValueCollection.AllKeys) {
            foreach (var interpreter in EnvironmentsInterpreter.Values) {
                if (!interpreter.Environment.VariableExists("GET." + key)) {
                    interpreter.Environment.CreateVariable("GET." + key, DataType.String,
                        new ValueExpression(nameValueCollection.Get(key), DataType.String));
                }
                else {
                    interpreter.Environment.UpdateVariable("GET." + key,
                        new ValueExpression(nameValueCollection.Get(key), DataType.String));
                }
            }
        }
    }
    public void RemoveGetRequestValues(NameValueCollection nameValueCollection) {
        foreach (var key in nameValueCollection.AllKeys) {
            foreach (var interpreter in EnvironmentsInterpreter.Values) {
                if (interpreter.Environment.VariableExists("GET." + key)) {
                    interpreter.Environment.UpdateVariable("GET." + key,
                        new ValueExpression(string.Empty, DataType.String));
                }
            }
        }
    }
}