using System;
using System.Collections.Generic;
using Cricket.Interpreter.Error;
using Cricket.Interpreter.Parser.Statement;

namespace Cricket.Interpreter.Parser;

/**
 * Klasa rozwiązywania.
 */
public class Resolver {
    private readonly List<IStatement> _statements;

    /**
     * Konstruktor klasy Resolver.
     * <param name="statements">Lista deklaracji</param>
     */
    public Resolver(List<IStatement> statements) {
        _statements = statements;
    }
    
    /**
     * Metoda rozpoczynająca proces rozwiązywania.
     */
    public void Resolve() {
        var environment = new ResolverEnvironment();
        foreach (var statement in _statements) statement.Resolve(environment);
    }

    /**
     * Klasa środowiska rozwiązywania.
     */
    public class ResolverEnvironment {
        private readonly List<ResolverFunction> _functions;
        private readonly ResolverEnvironment _parent;
        private readonly Dictionary<string, DataType> _variables;

        /**
         * Konstruktor klasy ResolverEnvironment.
         * <param name="parent">Rodzic środowiska</param>
         */
        public ResolverEnvironment(ResolverEnvironment parent) {
            _variables = new Dictionary<string, DataType>();
            _functions = parent == null ? new List<ResolverFunction>() : parent._functions;
            _parent = parent;
        }

        public ResolverEnvironment() : this(null) { }

        /**
         * Metoda sprawdzająca istnienie zmiennej.
         * <param name="name">Nazwa zmiennej</param>
         */
        public bool VariableExists(string name) {
            if (_variables.ContainsKey(name)) {
                return true;
            }
            return _parent?.VariableExists(name) ?? false;
        }

        /**
         * Metoda zwracająca typ zmiennej.
         * <param name="name">Nazwa zmiennej</param>
         */
        public DataType VariableReturns(string name) {
            return _variables.ContainsKey(name) ? _variables[name] : _parent.VariableReturns(name);
        }

        /**
         * Metoda dodająca zmienną do środowiska.
         * <param name="name">Nazwa zmiennej</param>
         * <param name="type">Typ zmiennej</param>
         */
        public void AddVariable(string name, DataType type) {
            _variables[name] = type;
        }
        
        /**
         * Metoda dodająca funkcję do środowiska.
         * <param name="name">Nazwa funkcji</param>
         * <param name="arguments">Lista typów argumentów</param>
         * <param name="returns">Typ zwracany przez funkcję</param>
         */
        public void AddFunction(string name, List<DataType> arguments, DataType returns) {
            var function = new ResolverFunction(name, arguments, returns);
            _functions.Add(function);
        }

        /**
         * Metoda sprawdzająca istnienie funkcji.
         * <param name="name">Nazwa funkcji</param>
         * <param name="arguments">Lista typów argumentów</param>
         */
        public bool FunctionExists(string name, List<DataType> arguments) {
            foreach (var function in _functions) {
                if (function.Name != name || function.Arguments.Count != arguments.Count) {
                    continue;
                }
                if (function.CompareArguments(arguments)) {
                    return true;
                }
            }
            return false;
        }

        /**
         * Metoda zwracająca typ wartości zwracanej przez funkcję.
         * <param name="name">Nazwa funkcji</param>
         * <param name="arguments">Lista typów argumentów funkcji</param>
         */
        public DataType FunctionReturns(string name, List<DataType> arguments) {
            foreach (var function in _functions) {
                if (function.Name != name || function.Arguments.Count != arguments.Count) {
                    continue;
                }
                if (function.CompareArguments(arguments)) {
                    return function.Returns;
                }
            }
            return DataType.Null;
        }

        /**
         * Metoda zwracająca środowisko globalne.
         * <returns>Środowisko rozwiązywania</returns>
         */
        public ResolverEnvironment GetGlobal() {
            return _parent == null ? this : _parent.GetGlobal();
        }
    }

    /**
     * Funkcja sprawdzająca integralność typu zmiennej.
     * <param name="name">Nazwa zmiennej</param>
     * <param name="present">Aktualny typ zmiennej</param>
     * <param name="required">Oczekiwany typ zmiennej</param>
     * <exception cref="ResolverError">Błąd integralności typów</exception>
     */
    public static void CheckTypeIntegrity(string name, DataType present, DataType required) {
        switch (required) {
            case DataType.String:
                if (present == DataType.String) return;
                break;
            case DataType.Integer:
                if (present == DataType.Integer) return;
                break;
            case DataType.Float:
                if (present is DataType.Integer or DataType.Float) return;
                break;
            case DataType.Boolean:
                if (present == DataType.Boolean) return;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        throw new ResolverError(
            @$"Value assigned to variable {name} has different type than defined. Required: {required}, present: {present}.");
    }

    /**
     * Klasa rozwiązywalnej funkcji.
     */
    private class ResolverFunction {
        public ResolverFunction(string name, List<DataType> arguments, DataType returns) {
            Name = name;
            Arguments = arguments;
            Returns = returns;
        }

        public string Name { get; }
        public List<DataType> Arguments { get; }
        public DataType Returns { get; }

        /**
         * Metoda porównująca listę typów zmiennych przekazanych do listy typów w funkcji.
         * <param name="arguments">Lista typów argumentów</param>
         * <returns>Czy argumenty są takie same</returns>
         */
        public bool CompareArguments(List<DataType> arguments) {
            for (var i = 0; i < arguments.Count; i++)
                if (Arguments[i] != arguments[i]) {
                    return false;
                }
            return true;
        }
    }
}