namespace Cricket.Interpreter.Environment;

public class VariableProperties
{
    public object Value { get; set; }
    public string Type { get; }

    public VariableProperties(object value, string type)
    {
        Value = value;
        Type = type;
    }
}