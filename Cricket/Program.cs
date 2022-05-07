namespace Cricket
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                var path = args[0];
                var interpreter = new Interpreter.Interpreter(path);
                interpreter.StartInterpreter();
            }
        }
    }
}