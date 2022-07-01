using System.Collections.Specialized;
using Cricket.Interpreter;
using Cricket.Interpreter.Parser.Statement.Expression;

namespace CricketBootstrap.Environment;

/**
 * Klasa zarządzająca środowiskami.
 */
public class EnvironmentManager {
    public Dictionary<string, Interpreter> EnvironmentsInterpreter { get; }

    /**
     * Konstruktor klasy EnvironmentManager.
     */
    public EnvironmentManager() {
        EnvironmentsInterpreter = new Dictionary<string, Interpreter>();
    }

    /**
     * Metoda dodająca wartości zapytania GET do środowisk.
     * <param name="nameValueCollection">Kolekcja wartości zapytania</param>
     */
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
    
    /**
     * Metoda usuwająca wartości zapytania GET ze środowisk.
     * <param name="nameValueCollection">Kolekcja wartości zapytania</param>
     */
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