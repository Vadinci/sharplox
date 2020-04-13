﻿using System;
using System.Collections.Generic;
using System.IO;

namespace SharpLox
{
    class Lox
    {
        static Boolean hadError = false;

        static void Main(string[] args)
        {
            Expr expression = new Expr.Binary(
                new Expr.Unary(
                    new Token(TokenType.MINUS, "-", null, 1),
                    new Expr.Literal(123)
                ),
                new Token(TokenType.STAR, "*", null, 1),
                new Expr.Grouping(new Expr.Literal(45.67))
            );

            AstPrinter printer = new AstPrinter();

            Console.Write(printer.Print(expression));



            return;

            if (args.Length > 1)
            {
                Console.Write("Usage: Lox [script]");
                return;
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }
        }



        private static void RunFile(string path)
        {
            string program = File.ReadAllText(path);
            Run(program);

            if (hadError) return;
        }

        private static void RunPrompt()
        {
            while (true)
            {
                Console.Write("|> ");
                Run(Console.ReadLine());
                hadError = false;
            }
        }

        private static void Run(string program)
        {
            Scanner scanner = new Scanner(program);
            List<Token> tokens = scanner.ScanTokens();

            foreach (Token t in tokens)
            {
                Console.Write(t);
            }
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.Error.Write("[Line " + line + "] Error" + where + ": " + message);
        }
    }
}
