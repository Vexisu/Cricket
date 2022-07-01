using System.Collections.Generic;
using Cricket.Interpreter.Parser.Statement;
using Cricket.Interpreter.Parser.Statement.Expression;

namespace Cricket.Interpreter.Environment;

/**
 * Klasa środowiska pracy.
 */
public class Environment {
    private readonly List<FunctionStatement> _functions;
    private readonly Environment _parent;
    private readonly Stack<IExpression> _stack;
    private readonly Dictionary<string, VariableWrapper> _variables;

    /**
     * Konstruktor klasy Environment.
     * <param name="parent">Środowisko rodzic</param>
     */
    public Environment(Environment parent) {
        _variables = new Dictionary<string, VariableWrapper>();
        _parent = parent;
        _functions = parent == null ? new List<FunctionStatement>() : parent._functions;
        _stack = parent == null ? new Stack<IExpression>() : parent._stack;
    }

    /**
     * Konstruktor domyślny klasy Environment.
     */
    public Environment() : this(null) { }

    /**
     * Metoda tworząca zmienną.
     * <param name="name">Nazwa zmiennej</param>
     * <param name="dataType">Typ wartości zmiennej</param>
     * <param name="expression">Wyrażenie przechowywane przez zmienną</param>
     */
    public bool CreateVariable(string name, DataType dataType, IExpression expression) {
        if (_variables.ContainsKey(name)) return false;
        _variables.Add(name, new VariableWrapper(expression, dataType));
        return true;
    }

    /**
     * Metoda aktualizująca wyrażenie zawarte w zmiennej.
     * <param name="name">Nazwa zmiennej</param>
     * <param name="expression">Wyrażenie docelowe</param>
     * <returns>Czy aktualizacja powiodła się</returns>
     */
    public bool UpdateVariable(string name, IExpression expression) {
        if (_variables.ContainsKey(name)) {
            _variables[name].Expression = expression;
            return true;
        }
        return _parent?.UpdateVariable(name, expression) ?? false;
    }

    /**
     * Metoda pobierająca wyrażenie przechowywane przez zmienną.
     * <param name="name">Nazwa zmiennej.</param>
     * <returns>Wyrażenie przechowywane przez zmienną</returns>
     */
    public IExpression GetVariable(string name) {
        return _variables.ContainsKey(name) ? _variables[name].Expression : _parent?.GetVariable(name);
    }

    /**
     * Metoda zwracająca typ wartości zwracanej przez zmienną.
     * <param name="name">Nazwa zmiennej</param>
     * <returns>Typ wartości zwracanej przez zmienną</returns>
     */
    public DataType GetVariableType(string name) {
        return _variables.ContainsKey(name)
            ? _variables[name].DataType
            : _parent?.GetVariableType(name) ?? DataType.Null;
    }

    /**
     * Metoda sprawdzająca czy zmienna istnieje.
     * <param name="name">Nazwa zmiennej</param>
     * <returns>Czy zmienna istnieje</returns>
     */
    public bool VariableExists(string name) {
        return _variables.ContainsKey(name) || (_parent?.VariableExists(name) ?? false);
    }

    /**
     * Metoda tworząca funkcję.
     * <param name="statement">Definicja funkcji</param>
     */
    public void CreateFunction(FunctionStatement statement) {
        _functions.Add(statement);
    }

    /**
     * Metoda wywołująca funkcję.
     * <param name="name">Nazwa funkcji</param>
     * <param name="argumentsType">Lista typów argumentów przyjmowanych przez funkcję</param>
     */
    public void CallFunction(string name, List<DataType> argumentsType) {
        foreach (var function in _functions) {
            if (function.Name != name || !CompareFunctionVariables(function, argumentsType)) continue;
            function.Call(this);
            return;
        }
    }

    /**
     * Metoda porównująca typy zmiennych przyjmowanych przez funkcję do przekazanych w liście.
     * <param name="function">Definicja funkcji</param>
     * <param name="argumentsType">Lista typów argumentów</param>
     * <returns>Czy typy zmiennych są takie same jak przyjmowane przez funkcję</returns>
     */
    private bool CompareFunctionVariables(FunctionStatement function, List<DataType> argumentsType) {
        if (argumentsType.Count != function.Arguments.Count) {
            return false;
        }
        for (var i = 0; i < argumentsType.Count; i++)
            if (argumentsType[i] != function.Arguments[i].Type) {
                return false;
            }
        return true;
    }

    /**
     * Metoda odkładająca wyrażenie na stos.
     * <param name="expression">Wyrażenie odkładane na stos</param>
     */
    public void PutOnStack(IExpression expression) {
        _stack.Push(expression);
    }

    /**
     * Metoda pobierająca wyrażenie ze stosu.
     * <returns>Wyrażenie pobrane ze stosu</returns>
     */
    public IExpression PopFromStack() {
        return _stack.Pop();
    }

    /**
     * Metoda pobierająca środowisko globalne.
     * <returns>Środowisko globalne</returns>
     */
    public Environment GetGlobal() {
        return _parent == null ? this : _parent.GetGlobal();
    }
}