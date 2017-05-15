
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class Token
    {
        public String TokenType { get; set; }
        public int Line { get; set; }
        public String Value { get; set; }

        public Token(String token, int line, String val)
        {
            TokenType = token;
            Line = line;
            Value = val;
        }
    }
}
