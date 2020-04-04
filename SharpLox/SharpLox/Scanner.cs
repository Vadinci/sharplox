using System;
using System.Collections.Generic;

namespace SharpLox
{
    public class Scanner
    {
        private readonly string source;
        private readonly List<Token> tokens = new List<Token>();

        private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
        {
            {"and", TokenType.AND},
            {"class",  TokenType.CLASS},
            {"else",   TokenType.ELSE},
            {"false",  TokenType.FALSE},
            {"for",    TokenType.FOR},
            {"fun",    TokenType.FUN},
            {"if",     TokenType.IF},
            {"nil",    TokenType.NIL},
            {"or",     TokenType.OR},
            {"print",  TokenType.PRINT},
            {"return", TokenType.RETURN},
            {"super",  TokenType.SUPER},
            {"this",   TokenType.THIS},
            {"true",   TokenType.TRUE},
            {"var",    TokenType.VAR},
            {"while",  TokenType.WHILE}
        };

        private int start = 0;
        private int current = 0;
        private int line = 1;

        public Scanner(string source)
        {
            this.source = source;
        }

        public List<Token> ScanTokens() {
            while (!IsAtEnd()) {
                start = current;
                ScanToken();
            }

            tokens.Add(new Token(TokenType.EOF, "", null, line));
            return tokens;
        }

        private bool IsAtEnd()
        {
            return current >= source.Length;
        }

        private void ScanToken() {
            char c = Advance();
            switch (c) {
                default:
                    if (IsDigit(c)) {
                        Number();
                        break;
                    }
                    if (IsAlpha(c)){
                        Identifier();
                        break;
                    }
                    Lox.Error(line, "Unexpect Character " + c);
                    break;

                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.STAR); break;
                case '!': AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG); break;
                case '=': AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL); break;
                case '<': AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS); break;
                case '>': AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER); break;
                case '/':
                    if (Match('/'))
                    {
                        //advance until we hit the end of the line, essentially ignoring the entire line from the moment we hit two slashes
                        while (!IsAtEnd() && Peek() != '\n') Advance();
                    }
                    else {
                        AddToken(TokenType.SLASH);
                    }
                    break;
                case ' ':
                case '\t':
                case '\r':
                    //ignore whitespace
                    break;
                case '\n':
                    line++;
                    break;
                case '"': String(); break;
            }
        }

        private char Advance() {
            current++;
            return source[current - 1];
        }

        private void AddToken(TokenType tokenType) {
            AddToken(tokenType, null);
        }

        private void AddToken(TokenType tokenType, Object literal) {
            string text = CurrentString();
            
            tokens.Add(new Token(tokenType, text, literal, line));
        }

        private bool Match(char expected) {
            if (IsAtEnd()) return false;
            if (source[current] != expected) return false;

            current++;
            return true;
        }

        private char Peek(int count = 0) {
            if (current+count >= source.Length) return '\0'; //end of file
            return source[current + count];
        }

        private string CurrentString() {
            return source.Substring(start, current - start);
        }

        private void String() {
            while (Peek() != '"' && !IsAtEnd()) {
                if (Peek() == '\n') line++;
                Advance();
            }

            if (IsAtEnd()) {
                Lox.Error(line, "unterminated String");
                return;
            }

            //add the closing "
            Advance();

            //Trim the surrounding quotation marks
            string value = source.Substring(start + 1, current - start - 2);
            AddToken(TokenType.STRING, value);
        }

        private void Number() {
            while (IsDigit(Peek())) {
                Advance();
            }

            if (Peek() == '.' && IsDigit(Peek(1))) {
                Advance();
                while (IsDigit(Peek()))
                {
                    Advance();
                }
            }

            AddToken(TokenType.NUMBER, Double.Parse(CurrentString()));
        }

        private void Identifier() {
            while (IsAlphaNumeric(Peek())) {
                Advance();
            }

            if (keywords.ContainsKey(CurrentString())) {
                AddToken(keywords[CurrentString()]);
                return;
            }
            //not a keyword
            AddToken(TokenType.IDENTIFIER);
        }

        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private bool IsAlpha(char c) {
            return c >= 'a' && c <= 'z' ||
                c >= 'A' && c <= 'Z' ||
                c == '_';
        }

        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }
    }
}
