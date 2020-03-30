using System;
using System.Collections.Generic;
using System.IO;

namespace SharpLox
{
    class Program
    {
        static Boolean hadError = false;

        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.Write("Usage: Lox [script]");
                return;
            }
            else if (args.Length == 1)
            {
                runFile(args[0]);
            }
            else {
                runPrompt();
            }
        }

        

        private static void runFile(string path)
        {
            string program = File.ReadAllText(path);
            run(program);

            if (hadError) return;
        }

        private static void runPrompt()
        {
            while (true) {
                Console.Write("|> ");
                run(Console.ReadLine());
                hadError = false;
            }
        }

        private static void run(string program)
        {
            Scanner scanner = new Scanner(program);
            List<Token> tokens = scanner.getTokens();

            for(Token t in tokens) {
                Console.Write(token);
            }
        }

        static void error(int line, string message) {
            report(line, "", message);
        }

        private static void report(int line, string where, string message) {
            Console.Error.Write("[Line "+line+ "] Error" + where + ": " + message);
        }
    }
}
