using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class Parser
    {
        List<Token> tokens;
        List<Token> iterator;
        Token currentToken;
        Token consumedToken;
        int position;

        public Parser(List<Token> lista)
        {
            this.tokens = lista;
            iterator = tokens.GetRange(0, tokens.Count);
            currentToken = tokens[0];
            consumedToken = currentToken;
            position = 0;
        }


        private void err(Token currentTkn, String stringToPrint)
        {
            Console.WriteLine(stringToPrint + " at " + currentTkn + " line:" + currentTkn.Line + "\n");
            
        }

        private bool consume(String code)
        {
            if (currentToken.TokenType.Equals(code))
            {
                consumedToken = currentToken;
                if (iterator[position+1] != null)
                {
                    currentToken = iterator[position + 1];
                    position++;
                }
                return true;
            }
            return false;
        }

        /*unit: ( declStruct | declFunc | declVar )* END ;*/
        public int unit()
        {
            while (true)
            {
                int p = position;
                List<Token> i = tokens.GetRange(position, tokens.Count);
                if (declStruct()) { }
                else
                     if (p != position) { iterator = i; currentToken = i[position+1]; position = p; }

                i = tokens.GetRange(position, tokens.Count - position);
                p = position;
                if (declVar()) { }
                else
                     if (p != position) { iterator = i; currentToken = i[position+1]; position = p; }
                if (declFunc()) { }
                else break;
            }
            if (consume("END")) { }
            else err(consumedToken, "missing END");
            return 1;
        }

        /*declStruct: STRUCT ID LACC declVar* RACC SEMICOLON ;*/
        private bool declStruct()
        {
            //System.out.println("#declStruct " + currentToken.getCode());
            if (!(consume("STRUCT"))) return false;
            if (!(consume("ID"))) err(consumedToken, "missing ID after STRUCT");
            if (!(consume("LACC"))) err(consumedToken, "missing ( after ID");

            while (true)
            {
                if (declVar()) { }
                else break;
            }
            if (!(consume("RACC"))) err(consumedToken, "missing )");
            if (!(consume("SEMICOLON"))) err(consumedToken, "missing ; after )");

            return true;
        }

        /*declFunc: ( typeBase MUL? | VOID ) ID  LPAR ( funcArg ( COMMA funcArg )* )? RPAR stmCompound ;*/
        private bool declFunc()
        {
            //System.out.println("#declFunc " + currentToken.getCode());
            //Iterator i = iterator;
            if (typeBase())
            {
                consume("MUL");
            }
            else if (!(consume("VOID")))
                return false;
            if (!(consume("ID"))) err(consumedToken, "missing ID");
            if (!(consume("LPAR"))) return false;
            if (funcArg())
                while (true)
                {
                    if ((consume("COMMA")))
                    { if ((funcArg())) { } }
                    else break;
                }
            if (!(consume("RPAR"))) err(consumedToken, "missing RPAR");
            if (!stmCompound()) return false;
            return true;
        }

        /*declVar:  typeBase ID arrayDecl? ( COMMA ID arrayDecl? )* SEMICOLON ;*/
        private bool declVar()
        {
            //System.out.println("#declVar " + currentToken.getCode());
            if (!(typeBase())) return false;
            if (!(consume("ID"))) err(consumedToken, "missing ID");
            arrayDecl();
            while (true)
            {
                if ((consume("COMMA")))
                {
                    if (!(consume("ID"))) err(consumedToken, "missing ID");
                    arrayDecl();
                }
                else break;
            }
            if (!(consume("SEMICOLON"))) err(consumedToken, "missing ;"); //return false;
            return true;
        }

        /*typeBase: INT | DOUBLE | CHAR | STRUCT ID ;*/
        private bool typeBase()
        {
            //System.out.println("#typeBase " + currentToken.getCode());
            if (consume("INT")) return true;
            if (consume("DOUBLE")) return true;
            if (consume("CHAR")) return true;
            if (consume("STRUCT"))
                if (consume("ID"))
                    return true;
                else err(consumedToken, "missing ID after STRUCT");
            return false;
        }

        /*arrayDecl: LBRACKET expr? RBRACKET ;*/
        private bool arrayDecl()
        {
            //System.out.println("#arrayDecl " + currentToken.getCode());
            if (!(consume("LBRACKET"))) return false;

            expr();

            if (!(consume("RBRACKET"))) err(consumedToken, "missing )");
            return true;
        }

        /*typeName: typeBase arrayDecl? ;*/
        private bool typeName()
        {
            //System.out.println("#typeName " + currentToken.getCode());
            if (!(typeBase())) return false;
            arrayDecl();
            return true;
        }

        /*expr: exprAssign ;*/
        private bool expr()
        {
            //System.out.println("#expr " + currentToken.getCode());
            if (!(exprAssign())) return false;
            return true;
        }

        /*funcArg: typeBase ID arrayDecl? ;*/
        private bool funcArg()
        {
            //System.out.println("#funcArg " + currentToken.getCode());
            if (!typeBase()) return false;
            if (!(consume("ID"))) err(consumedToken, "missing ID");
            arrayDecl();
            return true;
        }

        /*stmCompound: LACC ( declVar | stm )* RACC ;*/
        private bool stmCompound()
        {
            //System.out.println("#stmCompound " + currentToken.getCode());
            if (!(consume("LACC"))) return false;
            while (true)
            {
                if (declVar()) { }
                else if (stm()) { }
                else break;
            }
            if (!consume("RACC")) err(consumedToken, "missing RACC");
            return true;
        }

        /*stm: stmCompound 
               | IF LPAR expr RPAR stm ( ELSE stm )?
               | WHILE LPAR expr RPAR stm
               | FOR LPAR expr? SEMICOLON expr? SEMICOLON expr? RPAR stm
               | BREAK SEMICOLON
               | RETURN expr? SEMICOLON
               | expr? SEMICOLON ;*/
        private bool stm()
        {
            //System.out.println("#stm " + currentToken.getCode());
            if (consume("IF"))
            {
                if (!consume("LPAR")) err(consumedToken, "missing LPAR");
                if (!expr()) err(consumedToken, "missing expr");
                if (!consume("RPAR")) err(consumedToken, "missing RPAR");
                if (stm())
                {
                    if (consume("ELSE"))
                        if (!stm()) err(consumedToken, "missing stm");
                    return true;
                }
                else return false;
            }
            else if (consume("WHILE"))
            {
                if (!consume("LPAR")) err(consumedToken, "missing LPAR");
                if (!expr()) err(consumedToken, "missing expr");
                if (!consume("RPAR")) err(consumedToken, "missing RPAR");
                if (!stm()) return false;
                else return true;
            }
            else if (consume("FOR"))
            {
                if (!consume("LPAR")) err(consumedToken, "missing LPAR");
                expr();
                if (!consume("SEMICOLON")) err(consumedToken, "missing SEMICOLON");
                expr();
                if (!consume("SEMICOLON")) err(consumedToken, "missing SEMICOLON");
                expr();
                if (!consume("RPAR")) err(consumedToken, "missing RPAR");
                if (!stm()) return false;
                else return true;
            }
            else if (consume("BREAK"))
            {
                if (!consume("SEMICOLON")) err(consumedToken, "missing SEMICOLON");
                return true;
            }
            else if (consume("RETURN"))
            {
                expr();
                if (!consume("SEMICOLON")) err(consumedToken, "missing SEMICOLON");
                return true;
            }
            else if (consume("SEMICOLON")) return true;
            else if (expr())
            {
                if (!consume("SEMICOLON")) err(consumedToken, "missing SEMICOLON");
                return true;
            }
            else if (stmCompound()) return true;
            return false;
        }

        /*exprAssign: exprUnary ASSIGN exprAssign | exprOr ;*/
        private bool exprAssign()
        {
            //System.out.println("#exprAssign " + currentToken.getCode());
            /*if(exprOr()) return true;
            else currentToken=consumedToken;*/
            int p = position;
            List<Token> i = tokens.GetRange(position, tokens.Count - position);
            if (exprUnary())
            {
                if ((consume("ASSIGN"))) //err(consumedToken, "missing Assign");
                {
                    if (exprAssign()) return true;
                    else err(consumedToken, "error Assign");
                }
                else
                {

                    if (p != position) { iterator = i; currentToken = i[position+1]; position = p; }
                }


            }

            if (!exprOr()) return false;
            return true;
        }
            
        private bool exprOr()
        {
            //System.out.println("#exprOr " + currentToken.getCode());
            if (!exprAnd()) return false;
            if (!exprOr1()) return false;
            return true;
        }

        private bool exprOr1()
        {
            //System.out.println("#exprOr1 " + currentToken.getCode());
            if ((consume("OR")))
            {
                if (!(exprAnd())) return false;
                if (!(exprOr1())) return false;
            }
            return true;
        }

        /*exprAnd: exprAnd AND exprEq | exprEq ;*/
        private bool exprAnd()
        {
            //System.out.println("#exprAnd " + currentToken.getCode());
            if (!exprEq()) return false;
            if (!exprAnd1()) return false;
            return true;
        }

        private bool exprAnd1()
        {
           // System.out.println("#exprAnd1 " + currentToken.getCode());
            if ((consume("AND")))
            {
                if (!(exprEq())) return false;
                if (!(exprAnd1())) return false;
            }
            return true;
        }

        /*exprEq ( EQUAL | NOTEQ ) exprRel | exprRel ;*/
        private bool exprEq()
        {
            //System.out.println("#exprEq " + currentToken.getCode());
            if (!exprRel()) return false;
            if (!exprEq1()) return false;
            return true;
        }

        private bool exprEq1()
        {
            //System.out.println("#exprEq1 " + currentToken.getCode());
            if (consume("EQUAL") || consume("NOTEQ"))
            {
                if (!exprRel()) return false;
                if (!exprEq1()) return false;
            }
            return true;
        }

        /*exprRel: exprRel ( LESS | LESSEQ | GREATER | GREATEREQ ) exprAdd | exprAdd ;*/
        private bool exprRel()
        {
           // System.out.println("#exprRel " + currentToken.getCode());
            if (!exprAdd()) return false;
            if (!exprRel1()) return false;
            return true;
        }

        private bool exprRel1()
        {
            //System.out.println("#exprRel1 " + currentToken.getCode());
            if (consume("LESS") || consume("LESSEQ") || consume("GREATER") || consume("GREATEREQ"))
            {
                if (!exprAdd()) return false;
                if (!exprRel1()) return false;
            }
            return true;
        }

        /*exprAdd: exprAdd ( ADD | SUB ) exprMul | exprMul ;*/
        private bool exprAdd()
        {
            //System.out.println("#exprAdd " + currentToken.getCode());
            if (!exprMul()) return false;
            if (!exprAdd1()) return false;
            return true;
        }

        private bool exprAdd1()
        {
            //System.out.println("#exprAdd1 " + currentToken.getCode());
            if (consume("ADD") || consume("SUB"))
            {
                if (!exprMul()) return false;
                if (!exprAdd1()) return false;
            }
            return true;
        }

        /*exprMul: exprMul ( MUL | DIV ) exprCast | exprCast ;*/
        private bool exprMul()
        {
            //System.out.println("#exprMul " + currentToken.getCode());
            if (!exprCast()) return false;
            if (!exprMul1()) return false;
            return true;
        }

        private bool exprMul1()
        {
            //System.out.println("#exprMul1 " + currentToken.getCode());
            if (consume("MUL") || consume("DIV"))
            {
                if (!exprCast()) return false;
                if (!exprMul1()) return false;
            }
            return true;
        }

        /*exprCast: LPAR typeName RPAR exprCast | exprUnary ;*/
        private bool exprCast()
        {
            //System.out.println("#exprCast " + currentToken.getCode());
            if (!consume("LPAR"))
            { if (exprUnary()) return true; }
            else
            {
                if (typeName())// return false;
                    if (consume("RPAR")) //err(currentToken, "missing RPAR");
                        if (exprCast()) return true;
                        else return false;
                    else err(currentToken, "missing RPAR");
                else return false;
            }
            return false;
        }

        /*exprUnary: ( SUB | NOT ) exprUnary | exprPostfix ;*/
        private bool exprUnary()
        {
            //System.out.println("#exprUnary " + currentToken.getCode());
            if (!exprPostfix())
            {
                if (!consume("SUB") && !consume("NOT")) return false;
                if (!exprUnary()) return false;
            }
            return true;
        }

        private bool exprPostfix()
        {
            //System.out.println("#exprPostfix " + currentToken.getCode());
            if (exprPrimary())
            {
                exprPostFix1();
                return true;
            }

            return false;
        }

        private bool exprPostFix1()
        {
            //System.out.println("#exprPostFix1 " + currentToken.getCode());
            if (consume("DOT"))
            {
                if (consume("ID")) //return false;
                    if (exprPostFix1()) return true;
            }
            else if (consume("LBRACKET"))
            {
                if (expr()) //return false;
                    if (consume("RBRACKET")) //return false;
                        if (exprPostFix1()) return true;
            }
            return false;
        }


        /*exprPrimary: ID ( LPAR ( expr ( COMMA expr )* )? RPAR )?
               | CT_INT
               | CT_REAL 
               | CT_CHAR 
               | CT_STRING 
               | LPAR expr RPAR ;*/
        private bool exprPrimary()
        {
            //System.out.println("#exprPrimary " + currentToken.getCode());
            if (consume("ID"))
            {
                if (consume("LPAR"))
                {
                    if (expr())
                        while (true)
                        {
                            if (!consume("COMMA")) break;
                            else
                                    if (expr()) { }
                        }
                    if (!consume("RPAR")) err(consumedToken, "missing RPAR");
                }
                return true;
            }
            else if (consume("CT_INT")) return true;
            else if (consume("CT_REAL")) return true;
            else if (consume("CT_CHAR")) return true;
            else if (consume("CT_STRING")) return true;
            else if (consume("LPAR"))
            {
                if (expr())
                    if (consume("RPAR")) return true;
                    else err(consumedToken, "missing RPAR");
                else err(consumedToken, "missing expr");
            }
            return false;
        }
    }

}
