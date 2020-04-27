using System;
using System.Collections.Generic;

namespace SharpLox
{
    public class Parser
    {
        private readonly List<Token> tokens;
        private int current = 0;

        private class ParseError : Exception
        {

        }

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;

        }

        public Expr Parse()
        {
            try
            {
                return Expression();
            }
            catch (Exception e) {
                return null;
            }
        }


        private bool Match(params TokenType[] tokenTypes)
        {
            for (int ii = 0; ii < tokenTypes.Length; ii++)
            {
                if (Check(tokenTypes[ii]))
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }

        private bool Check(TokenType tokenType)
        {
            if (IsAtEnd()) return false;
            return Peek().type == tokenType;
        }

        private Token Advance()
        {
            if (!IsAtEnd()) current++;
            return Previous();
        }

        private Token Previous()
        {
            return tokens[current - 1];
        }

        private bool IsAtEnd()
        {
            return Peek().type == TokenType.EOF;
        }

        private Token Peek()
        {
            return tokens[current];
        }

        private Token Consume(TokenType tokenType, string errorMsg)
        {
            if (Check(tokenType)) return Advance();

            throw Error(Peek(), errorMsg);
        }



        private ParseError Error(Token token, string message)
        {
            Lox.Error(token, message);
            return new ParseError();
        }

        private void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().type == TokenType.SEMICOLON) return;

                switch (Peek().type)
                {
                    case TokenType.CLASS:
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }

                Advance();
            }
        }



        //expression -> equality 
        private Expr Expression()
        {
            return Equality();
        }

        //equality -> comparison ( ("!=" | "==") comparison)*;
        private Expr Equality()
        {
            Expr expr = Comparison();

            while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                Token op = Previous();
                Expr right = Comparison();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        //comparison -> addition ( (">" | ">=" | "<" | "<=") addition)*;
        private Expr Comparison()
        {
            Expr expr = Addition();

            while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                Token op = Previous();
                Expr right = Addition();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        //addition -> multiplication ( ("-" | "+") multiplication)*;
        private Expr Addition()
        {
            Expr expr = Multiplication();

            while (Match(TokenType.MINUS, TokenType.PLUS))
            {
                Token op = Previous();
                Expr right = Multiplication();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        //multiplication -> unary ( ("*" | "/") unary)*;
        private Expr Multiplication()
        {
            Expr expr = Unary();

            while (Match(TokenType.STAR, TokenType.SLASH))
            {
                Token op = Previous();
                Expr right = Unary();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        //multiplication ->  ("!" | "-") unary | primary;
        private Expr Unary()
        {
            if (Match(TokenType.BANG, TokenType.MINUS))
            {
                Token op = Previous();
                Expr right = Unary();
                return new Expr.Unary(op, right);
            }

            return Primary();
        }

        //primary -> NUMBER | STRING | "false" | "true" | "nil" | "(" expression ")" ;
        private Expr Primary()
        {
            if (Match(TokenType.FALSE)) return new Expr.Literal(false);
            if (Match(TokenType.TRUE)) return new Expr.Literal(true);
            if (Match(TokenType.NIL)) return new Expr.Literal(null);

            if (Match(TokenType.NUMBER, TokenType.STRING))
            {
                return new Expr.Literal(Previous().literal);
            }

            if (Match(TokenType.LEFT_PAREN))
            {
                Expr expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expected ')' after expression");
                return new Expr.Grouping(expr);
            }

            throw Error(Peek(), "Expect expression.");
        }
    }
}
