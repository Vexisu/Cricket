using System;
using System.Collections.Generic;

namespace Cricket.Interpreter.Scanner
{
    public class Scanner
    {
        private string[] _source;
        private int index = 0, line = 0;

        public Scanner(string[] source)
        {
            _source = source;
        }

        public List<Token> Tokenize()
        {
            var tokens = new List<Token>();
            while (!EndOfFile())
            {
                var current = Consume();
                
            }
            return null;
        }

        private char Consume()
        {
            var character = _source[line][index];
            if (_source[line].Length == ++index)
            {
                index = 0;
                line++;
            }
            return character;
        }

        private bool EndOfFile()
        {
            return line == _source.Length;
        }
    }
}