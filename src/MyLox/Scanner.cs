namespace MyLox;

public class Scanner
{
    private readonly string _source;
    private readonly List<Token> _tokens = [];
    private int _start;
    private int _current;
    private int _line = 1;

    private static readonly Dictionary<string, TokenType> Keywords = new()
    {
        ["and"] = TokenType.And,
        ["class"] = TokenType.Class,
        ["else"] = TokenType.Else,
        ["false"] = TokenType.False,
        ["for"] = TokenType.For,
        ["fun"] = TokenType.Fun,
        ["if"] = TokenType.If,
        ["nil"] = TokenType.Nil,
        ["or"] = TokenType.Or,
        ["print"] = TokenType.Print,
        ["return"] = TokenType.Return,
        ["super"] = TokenType.Super,
        ["this"] = TokenType.This,
        ["true"] = TokenType.True,
        ["var"] = TokenType.Var,
        ["while"] = TokenType.While
    };

    public Scanner(string source)
    {
        _source = source;
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            _start = _current;
            ScanToken();
        }

        _tokens.Add(new Token(TokenType.Eof, "", null, _line));
        return _tokens;
    }

    private void ScanToken()
    {
        char c = Advance();
        switch (c)
        {
            case '(': AddToken(TokenType.LeftParen); break;
            case ')': AddToken(TokenType.RightParen); break;
            case '{': AddToken(TokenType.LeftBrace); break;
            case '}': AddToken(TokenType.RightBrace); break;
            case ',': AddToken(TokenType.Comma); break;
            case '.': AddToken(TokenType.Dot); break;
            case '-': AddToken(TokenType.Minus); break;
            case '+': AddToken(TokenType.Plus); break;
            case ';': AddToken(TokenType.Semicolon); break;
            case '*': AddToken(TokenType.Star); break;
            case '!':
                AddToken(Match('=') ? TokenType.BangEqual : TokenType.Bang);
                break;
            case '=':
                AddToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal);
                break;
            case '<':
                AddToken(Match('=') ? TokenType.LessEqual : TokenType.Less);
                break;
            case '>':
                AddToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater);
                break;
            case '/':
                if (Match('/'))
                {
                    while (Peek() != '\n' && !IsAtEnd())
                    {
                        Advance();
                    }
                }
                else                                                                                            
                {
                    AddToken(TokenType.Slash);
                }

                break;
            case ' ':
            case '\r':
            case '\t':
                // Ignore whitespace
                break;
            case '\n':
                _line++;
                break;
            case '"':
                String();
                break;
            default:
                if (IsDigit(c))
                {
                    Number();
                }
                else if (IsAlpha(c))
                {
                    Identifier();
                }
                else
                {
                    MyLox.Error(_line, "Unexpected character.");
                }
                break;
        }
    }

    private void Identifier()
    {
        while (IsAlphanumeric(Peek()))
        {
            Advance();
        }

        string text = _source[_start.._current];
        try
        {
            AddToken(Keywords[text]);
        }
        catch (KeyNotFoundException)
        {
            AddToken(TokenType.Identifier);
        }
    }

    private void Number()
    {
        while (IsDigit(Peek()))
        {
            Advance();
        }
        
        // Look for a fractional part
        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            // Consume the "."
            Advance();

            while (IsDigit(Peek()))
            {
                Advance();
            }
        }
        
        AddToken(TokenType.Number, Convert.ToDouble(_source[_start.._current]));
    }

    private void String()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n')
            {
                _line++;
            }

            Advance();
        }

        if (IsAtEnd())
        {
            MyLox.Error(_line, "Unterminated string.");
            return;
        }
        
        // The closing ""
        Advance();

        // Trim the surrounding quotes
        string value = _source[(_start + 1)..(_current - 1)];
        AddToken(TokenType.String, value);
    }

    private bool Match(char expected)
    {
        if (IsAtEnd())
        {
            return false;
        }

        if (_source[_current] != expected)
        {
            return false;
        }

        _current++;
        return true;
    }

    private char Peek()
    {
        return IsAtEnd() ? '\0' : _source[_current];
    }

    private char PeekNext()
    {
        if (_current + 1 >= _source.Length)
        {
            return '\0';
        }

        return _source[_current + 1];
    }

    private static bool IsAlpha(char c)
    {
        return (c >= 'a' && c <= 'z') ||
               (c >= 'A' && c <= 'Z') ||
               c == '_';
    }

    private static bool IsAlphanumeric(char c)
    {
        return IsAlpha(c) || IsDigit(c);
    }

    private static bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    private bool IsAtEnd()
    {
        return _current >= _source.Length;
    }

    private char Advance()
    {
        return _source[_current++];
    }

    private void AddToken(TokenType type)
    {
        AddToken(type, null);
    }

    private void AddToken(TokenType type, object literal)
    {
        string text = _source[_start.._current];
        _tokens.Add(new Token(type, text, literal, _line));
    }
}
