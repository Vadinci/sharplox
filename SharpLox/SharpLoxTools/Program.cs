using System;

namespace SharpLoxTools
{
    class Program
    {
        static void Main(string[] args)
        {
            GenerateAST.Run(args[0]);
        }
    }
}
