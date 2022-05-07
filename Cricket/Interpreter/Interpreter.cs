namespace Cricket.Interpreter
{
    public class Interpreter
    {
        private readonly string _path;

        public Interpreter(string path)
        {
            _path = path;
        }

        public void StartInterpreter()
        {
            var source = System.IO.File.ReadAllLines(_path);
            var scanner = new Scanner.Scanner(source);
            scanner.Tokenize();
        }
    }
}