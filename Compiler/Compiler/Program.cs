using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            Lexor lexor = new Lexor("9.c");
            Parser parser;

            List<Token> tokenList = new List<Token>();

            while(true)
            {
                var token = lexor.getToken();

                if (token == null)
                    break;
                else
                    tokenList.Add(token);

            }

            tokenList.Add(new Token("END", 0, ""));

            parser = new Parser(tokenList);
            parser.unit();

        }
    }
}
