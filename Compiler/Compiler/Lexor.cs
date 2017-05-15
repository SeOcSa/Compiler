using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class Lexor
    {

        //	private List<Token> lista = new ArrayList<Token>();

        private StreamReader reader; // Reader
        private char current_ch; // The current character being scanned
        private int linie = 1;

        List<Token> tokenList = new List<Token>();

        public Lexor(String file)
        {
            try
            {
                reader = new StreamReader(file);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Read the first character
            current_ch = read();
        }

        private char read()
        {
            try
            {
                return (char)(reader.Read());
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                return '\uffff';
            }
        }

        private bool isDigit(char c)
        {
            if (c >= '0' && c <= '9')
                return true;

            return false;
        }

        public bool isAlpha(char c)
        {
            if (c >= 'a' && c <= 'z')
                return true;
            if (c >= 'A' && c <= 'Z')
                return true;

            return false;

        }

        public Token getToken()
        {
            /*public void GetNextToken(){*/
            int state = 0;
            String number = "";
            String id_name = "";
            String ct_string = "";
            String ct_char = "";
            bool skipped = false;
            while (true)
            {

                if (current_ch == '\uffff' && !skipped)
                {
                    skipped = true;
                }
                else if (skipped)
                {
                    try
                    {
                        reader.Close();
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    return null;
                }
                switch (state)
                {
                    case 0:

                        if (current_ch == '\n')
                        {
                            linie++;
                            current_ch = read();
                        }

                        else if (current_ch == ' ' || current_ch == '\b' || current_ch == '\f' || current_ch == '\r' || current_ch == '\t')
                            current_ch = read();

                        else if (current_ch == ',')
                        {
                            current_ch = read();
                            state = 16;
                        }
                        else if (current_ch == ';')
                        {
                            current_ch = read();
                            state = 17;
                        }
                        else if (current_ch == '(')
                        {
                            current_ch = read();
                            state = 18;
                        }
                        else if (current_ch == ')')
                        {
                            current_ch = read();
                            state = 19;
                        }
                        else if (current_ch == '[')
                        {
                            current_ch = read();
                            state = 20;
                        }
                        else if (current_ch == ']')
                        {
                            current_ch = read();
                            state = 21;
                        }
                        else if (current_ch == '{')
                        {
                            current_ch = read();
                            state = 22;
                        }
                        else if (current_ch == '}')
                        {
                            current_ch = read();
                            state = 23;
                        }
                        else if (current_ch == '+')
                        {
                            current_ch = read();
                            state = 24;
                        }
                        else if (current_ch == '-')
                        {
                            current_ch = read();
                            state = 25;
                        }
                        else if (current_ch == '*')
                        {
                            current_ch = read();
                            state = 26;
                        }
                        else if (current_ch == '.')
                        {
                            current_ch = read();
                            state = 27;
                        }
                        else if (current_ch == '&')
                        {
                            current_ch = read();
                            state = 28;
                        }
                        else if (current_ch == '|')
                        {
                            current_ch = read();
                            state = 30;
                        }
                        else if (current_ch == '!')
                        {
                            current_ch = read();
                            state = 32;
                        }
                        else if (current_ch == '=')
                        {
                            current_ch = read();
                            state = 35;
                        }
                        else if (current_ch == '<')
                        {
                            current_ch = read();
                            state = 38;
                        }
                        else if (current_ch == '>')
                        {
                            current_ch = read();
                            state = 41;
                        }
                        else if (current_ch >= '1' && current_ch <= '9')
                        {
                            number += current_ch;
                            current_ch = read();
                            state = 1;
                        }
                        else if (current_ch == '0') { number += current_ch; current_ch = read(); state = 3; }
                        else if (isAlpha(current_ch)) { id_name += current_ch; current_ch = read(); state = 14; }
                        else if (current_ch == '"') { current_ch = read(); state = 49; }
                        else if (current_ch == '\'') { current_ch = read(); state = 52; }
                        else if (current_ch == '/') { current_ch = read(); state = 44; }
                        continue;

                    case 1:
                        if (isDigit(current_ch))
                        {
                            number += current_ch;
                            state = 1;
                            current_ch = read();
                        }
                        else if (current_ch == '.') { number += current_ch; state = 8; current_ch = read(); }
                        else if (current_ch == 'E' || current_ch == 'e') { number += current_ch; state = 10; current_ch = read(); }
                        else state = 2;
                        break;

                    case 2:
                        //return new Token("CT_INT",linie,number);

                        if (number.StartsWith("0x"))
                        {
                            number = number.Substring(2);
                            int a = int.Parse(number, System.Globalization.NumberStyles.HexNumber);

                            //return new IntToken("CT_INT",linie,a);
                            return new Token("CT_INT", linie, a.ToString());
                        }
                        else if (number.StartsWith("0"))
                        {
                            if (number.Length != 1)
                            {
                                number = number.Substring(1);
                                int a = Convert.ToInt32(number, 8);

                                //return new IntToken("CT_INT",linie,a);
                                return new Token("CT_INT", linie, a.ToString());
                            }
                            //else return new IntToken("CT_INT",linie,Integer.parseInt(number));
                            else return new Token("CT_INT", linie, number);
                        }
                        //return new IntToken("CT_INT",linie,Integer.parseInt(number));
                        return new Token("CT_INT", linie, number);
                    case 3:
                        if (current_ch == 'x')
                        {
                            number += current_ch;
                            state = 5;
                            current_ch = read();
                        }
                        else if (current_ch == '8' || current_ch == '9')
                        {
                            number += current_ch;
                            state = 7;
                            current_ch = read();
                        }
                        else if (current_ch == 'e' || current_ch == 'E')
                        {
                            number += current_ch;
                            state = 10;
                            current_ch = read();
                        }
                        else
                        {
                            state = 4;
                        }
                        break;

                    case 4:
                        if (current_ch >= '0' && current_ch <= '7')
                        {
                            number += current_ch;
                            state = 4;
                            current_ch = read();
                        }
                        else if (current_ch == '.')
                        {
                            number += current_ch;
                            state = 8;
                            current_ch = read();
                        }
                        else if (current_ch == 'e' || current_ch == 'E')
                        {
                            number += current_ch;
                            state = 10;
                            current_ch = read();
                        }
                        else state = 2;
                        break;

                    case 5:
                        if ((current_ch >= '0' && current_ch <= '9') || (current_ch >= 'a' && current_ch <= 'f') ||
                                (current_ch >= 'A' && current_ch <= 'F'))
                        {
                            number += current_ch;
                            state = 6;
                            current_ch = read();
                        }
                        break;

                    case 6:
                        if ((current_ch >= '0' && current_ch <= '9') || (current_ch >= 'a' && current_ch <= 'f') ||
                                (current_ch >= 'A' && current_ch <= 'F'))
                        {
                            number += current_ch;
                            state = 6;
                            current_ch = read();
                        }
                        else state = 2;
                        break;

                    case 7:
                        if (isDigit(current_ch))
                        {
                            number += current_ch;
                            state = 7;
                            current_ch = read();
                        }
                        else if (current_ch == '.')
                        {
                            number += current_ch;
                            state = 8;
                            current_ch = read();
                        }
                        break;

                    case 8:
                        if (isDigit(current_ch))
                        {
                            number += current_ch;
                            state = 9;
                            current_ch = read();
                        }
                        break;

                    case 9:
                        if (isDigit(current_ch))
                        {
                            number += current_ch;
                            state = 9;
                            current_ch = read();
                        }
                        else if (current_ch == 'e' || current_ch == 'E')
                        {
                            number += current_ch;
                            state = 10;
                            current_ch = read();
                        }
                        else state = 13;
                        break;

                    case 10:
                        if (isDigit(current_ch))
                        {
                            number += current_ch;
                            state = 12;
                            current_ch = read();
                        }
                        else if (current_ch == '+' || current_ch == '-')
                        {
                            number += current_ch;
                            state = 11;
                            current_ch = read();
                        }
                        break;

                    case 11:
                        if (isDigit(current_ch))
                        {
                            number += current_ch;
                            state = 12;
                            current_ch = read();
                        }
                        break;

                    case 12:
                        if (isDigit(current_ch))
                        {
                            number += current_ch;
                            state = 12;
                            current_ch = read();
                        }
                        else state = 13;
                        break;

                    case 13:
                        //				return new Token("CT_REAL",linie,number);
                        //return new DoubleToken("CT_REAL",linie,Double.parseDouble(number));
                        return new Token("CT_REAL", linie, Double.Parse(number).ToString());
                    case 14:
                        if (isDigit(current_ch) || isAlpha(current_ch) || current_ch == '_')
                        {
                            id_name += current_ch;
                            state = 14;
                            current_ch = read();
                        }
                        else state = 15;
                        break;

                    case 15:
                        if (id_name.ToUpper().Equals("INT")) return new Token("INT", linie, "");
                        if (id_name.ToUpper().Equals("BREAK")) return new Token("BREAK", linie, "");
                        if (id_name.ToUpper().Equals("CHAR")) return new Token("CHAR", linie, "");
                        if (id_name.ToUpper().Equals("DOUBLE")) return new Token("DOUBLE", linie, "");
                        if (id_name.ToUpper().Equals("ELSE")) return new Token("ELSE", linie, "");
                        if (id_name.ToUpper().Equals("FOR")) return new Token("FOR", linie, "");
                        if (id_name.ToUpper().Equals("IF")) return new Token("IF", linie, "");
                        if (id_name.ToUpper().Equals("RETURN")) return new Token("RETURN", linie, "");
                        if (id_name.ToUpper().Equals("STRUCT")) return new Token("STRUCT", linie, "");
                        if (id_name.ToUpper().Equals("VOID")) return new Token("VOID", linie, "");
                        if (id_name.ToUpper().Equals("WHILE")) return new Token("WHILE", linie, "");

                        return new Token("ID", linie, id_name);

                    case 16:
                        //lista.add(new Token("COMMA",linie,""));
                        return new Token("COMMA", linie, "");
                    //				break;
                    case 17:
                        /*lista.add(new Token("SEMICOLON",linie,""));*/
                        return new Token("SEMICOLON", linie, "");
                    //				break;
                    case 18:
                        /*lista.add(new Token("LPAR",linie,""));*/
                        return new Token("LPAR", linie, "");
                    //				break;
                    case 19:
                        /*lista.add(new Token("RPAR",linie,""));*/
                        return new Token("RPAR", linie, "");
                    //				break;
                    case 20:
                        /*lista.add(new Token("LBRACKET",linie,""));*/
                        return new Token("LBRACKET", linie, "");
                    //				break;
                    case 21:
                        /*lista.add(new Token("RBRACKET",linie,""));*/
                        return new Token("RBRACKET", linie, "");
                    //				break;
                    case 22:
                        /*lista.add(new Token("LACC",linie,""));*/
                        return new Token("LACC", linie, "");
                    //				break;
                    case 23:
                        /*lista.add(new Token("RACC",linie,""));*/
                        return new Token("RACC", linie, "");
                    //				break;
                    case 24:
                        /*lista.add(new Token("ADD",linie,""));*/
                        return new Token("ADD", linie, "");
                    //				break;
                    case 25:
                        /*lista.add(new Token("SUB",linie,""));*/
                        return new Token("SUB", linie, "");
                    //				break;
                    case 26:
                        /*lista.add(new Token("MUL",linie,""));*/
                        return new Token("MUL", linie, "");
                    //				break;
                    case 27:
                        /*lista.add(new Token("DOT",linie,""));*/
                        return new Token("DOT", linie, "");
                    //				break;
                    case 28:
                        //current_ch=read();	
                        state = 29;
                        continue;
                    case 29:
                        if (current_ch == '&')
                        {
                            current_ch = read();
                            /*lista.add(new Token("AND",linie,""));*/
                            return new Token("AND", linie, "");
                        }
                        continue;
                    case 30:
                        //current_ch=read();	
                        state = 31;
                        continue;
                    case 31:
                        if (current_ch == '|')
                        {
                            current_ch = read();
                            /*lista.add(new Token("OR",linie,""));*/
                            return new Token("OR", linie, "");
                        }
                        continue;
                    case 32:
                        if (current_ch == '=') state = 34;
                        else state = 33;
                        continue;
                    case 33:
                        /*lista.add(new Token("NOT",linie,""));*/
                        return new Token("NOT", linie, "");
                    //				continue;
                    case 34:
                        current_ch = read();
                        /*lista.add(new Token("NOTEQ",linie,""));*/
                        return new Token("NOTEQ", linie, "");
                    //				continue;
                    case 35:
                        if (current_ch == '=') state = 36;
                        else state = 37;
                        continue;
                    case 36:
                        current_ch = read();
                        /*lista.add(new Token("EQUAL",linie,""));*/
                        return new Token("EQUAL", linie, "");
                    //				continue;
                    case 37:
                        /*lista.add(new Token("ASSIGN",linie,""));*/
                        return new Token("ASSIGN", linie, "");
                    //				continue;
                    case 38:
                        if (current_ch == '=') state = 39;
                        else state = 40;
                        continue;
                    case 39:
                        current_ch = read();
                        /*lista.add(new Token("LESSEQ",linie,""));*/
                        return new Token("LESSEQ", linie, "");
                    //				continue;
                    case 40:
                        /*lista.add(new Token("LESS",linie,""));*/
                        return new Token("LESS", linie, "");
                    //				continue;
                    case 41:
                        if (current_ch == '=') state = 42;
                        else state = 43;
                        continue;
                    case 42:
                        current_ch = read();
                        /*lista.add(new Token("GREATEREQ",linie,""));*/
                        return new Token("GREATEREQ", linie, "");
                    //				continue;
                    case 43:
                        /*lista.add(new Token("GREATER",linie,""));*/
                        return new Token("GREATER", linie, "");
                    //				continue;

                    case 44:
                        if (current_ch == '/')
                        {
                            current_ch = read();
                            state = 46;
                        }
                        else if (current_ch == '*')
                        {
                            current_ch = read();
                            state = 47;
                        }
                        else state = 45;
                        break;

                    case 45:
                        //current_ch=read();
                        return new Token("DIV", linie, "");

                    case 46:
                        if (current_ch != '\n' && current_ch != '\r' && current_ch != '\0')
                        {
                            state = 46;
                            current_ch = read();
                        }
                        else state = 0;
                        break;

                    case 47:
                        if (current_ch != '*')
                        {
                            state = 47;
                            current_ch = read();
                        }
                        else
                        {
                            state = 48;
                            current_ch = read();
                        }
                        break;

                    case 48:
                        if (current_ch != '*' && current_ch != '/')
                        {
                            state = 47;
                            current_ch = read();
                        }
                        else if (current_ch == '*')
                        {
                            state = 48;
                            current_ch = read();
                        }
                        else if (current_ch == '/')
                        {
                            state = 0;
                            current_ch = read();
                        }
                        break;

                    case 49:
                        if (current_ch == '\\')
                        {

                            state = 51;
                            //ct_string+=current_ch;
                            current_ch = read();
                        }
                        else if (current_ch != '"' && current_ch != '\\')
                        {
                            ct_string += current_ch;
                            current_ch = read();
                            state = 49;

                        }
                        else if (current_ch == '"')
                        {
                            current_ch = read();
                            state = 50;
                        }
                        break;

                    case 50:
                        return new Token("CT_STRING", linie, ct_string);

                    case 51:
                        switch (current_ch)
                        {
                            case 'b':
                                ct_string += '\b';
                                current_ch = read();
                                state = 49;
                                break;
                            case 'f':
                                ct_string += '\f';
                                current_ch = read();
                                state = 49;
                                break;
                            case 'n':
                                ct_string += '\n';
                                current_ch = read();
                                state = 49;
                                break;
                            case 'r':
                                ct_string += '\r';
                                current_ch = read();
                                state = 49;
                                break;
                            case 't':
                                ct_string += '\t';
                                current_ch = read();
                                state = 49;
                                break;
                            case '\'':
                                ct_string += '\'';
                                current_ch = read();
                                state = 49;
                                break;
                            case '?':
                                ct_string += '?';
                                current_ch = read();
                                state = 49;
                                break;
                            case '"':
                                ct_string += '"';
                                current_ch = read();
                                state = 49;
                                break;
                            case '0':
                                ct_string += '\0';
                                current_ch = read();
                                state = 49;
                                break;
                            case '\\':
                                ct_string += '\\';
                                current_ch = read();
                                state = 49;
                                break;
                        }
                        break;

                    case 52:
                        if (current_ch == '\\')
                        {
                            state = 53;
                            ct_char += '\\';
                            current_ch = read();
                        }
                        else if (current_ch != '\'' && current_ch != '\\')
                        {
                            state = 56;
                            ct_char += current_ch;
                            current_ch = read();
                        }
                        break;

                    case 53:
                        switch (current_ch)
                        {
                            case 'b':
                                ct_string += '\b';
                                current_ch = read();
                                state = 54;
                                break;
                            case 'f':
                                ct_string += '\f';
                                current_ch = read();
                                state = 54;
                                break;
                            case 'n':
                                ct_string += '\n';
                                current_ch = read();
                                state = 54;
                                break;
                            case 'r':
                                ct_string += '\r';
                                current_ch = read();
                                state = 54;
                                break;
                            case 't':
                                ct_string += '\t';
                                current_ch = read();
                                state = 54;
                                break;
                            case '\'':
                                ct_string += '\'';
                                current_ch = read();
                                state = 54;
                                break;
                            case '?':
                                ct_string += '?';
                                current_ch = read();
                                state = 54;
                                break;
                            case '"':
                                ct_string += '"';
                                current_ch = read();
                                state = 54;
                                break;
                            case '0':
                                ct_string += '\0';
                                current_ch = read();
                                state = 54;
                                break;
                            case '\\':
                                ct_string += '\\';
                                current_ch = read();
                                state = 54;
                                break;
                        }
                        break;

                    case 54:
                        if (current_ch == '\'')
                        {
                            current_ch = read();
                            state = 55;
                        }
                        break;

                    case 55:
                        return new Token("CT_CHAR", linie, ct_char);

                    case 56:
                        if (current_ch == '\'')
                        {
                            state = 55;
                            current_ch = read();
                        }
                        break;
                }
            }
        }
    }
}
