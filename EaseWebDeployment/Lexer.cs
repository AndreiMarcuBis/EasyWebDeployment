using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EaseWebDeployment
{
    class Lexer
    {
        public List<Token> lex(string s)
        {
            List<Token> tokens = new List<Token>();
            tokens.Add(new Token("START_DOCUMENT"));

            int i = 0;

            while (true)
            {
                string last_token_name = tokens.Last().get_name();

                if (last_token_name == "END_DOCUMENT")
                {
                    break;
                }
                if (last_token_name == "START_DOCUMENT")
                {
                    parse_doctype(s, ref i, tokens);
                }
                else if (last_token_name == "HTML5_DOCTYPE" || last_token_name == "END_TAG" || last_token_name == "END_CLOSING_TAG")
                {
                    parse_content(s, ref i, tokens);
                }
                else if (last_token_name == "ANY_TAG")
                {
                    parse_any_tag(s, ref i, tokens);
                }
                else if (last_token_name == "START_TAG")
                {
                    parse_tag_name(s, ref i, tokens);
                }
                else if (last_token_name == "TAG_NAME" || last_token_name == "ATTRIBUTE_NAME" || last_token_name == "ATTRIBUTE_VALUE")
                {
                    parse_tag_attribute_name(s, ref i, tokens);
                }
                else if (last_token_name == "ATTRIBUTE_ASSIGNATION")
                {
                    parse_tag_attribute_value(s, ref i, tokens);
                }
                else if (last_token_name == "START_CLOSING_TAG")
                {
                    parse_closing_tag(s, ref i, tokens);
                }
            }

            return tokens;
        }

        private void parse_doctype(string content, ref int i, List<Token> tokens)
        {
            i = content.IndexOf("\n");
            if (i == -1)
            {
                i = content.Length;
                tokens.Add(new Token("INVALID_DOCUMENT"));
                tokens.Add(new Token("END_DOCUMENT"));
            }
            else
            {
                i += 1;
                tokens.Add(new Token("HTML5_DOCTYPE"));
            }
        }

        private void parse_content(string content, ref int i, List<Token> tokens)
        {
            string b = "";

            while (i < content.Length)
            {
                char c = content[i];
                i += 1;

                if (c == '<')
                {
                    tokens.Add(new Token("CONTENT", b));
                    tokens.Add(new Token("ANY_TAG"));

                    return;
                }
                else
                {
                    b += c;
                }
            }

            tokens.Add(new Token("CONTENT", b));
            tokens.Add(new Token("END_DOCUMENT"));
        }

        private void parse_any_tag(string content, ref int i, List<Token> tokens)
        {
            char c = content[i];

            tokens.RemoveAt(tokens.Count - 1);

            if (c == '/')
            {
                i += 1;
                tokens.Add(new Token("START_CLOSING_TAG"));
            }
            else
            {
                tokens.Add(new Token("START_TAG"));
            }
        }

        private void parse_tag_name(string content, ref int i, List<Token> tokens)
        {
            string b = "";
            while (i < content.Length)
            {
                char c = content[i];
                i += 1;

                if (c == '>')
                {
                    tokens.Add(new Token("TAG_NAME", b));
                    tokens.Add(new Token("END_TAG"));

                    return;
                }
                else if (c == ' ')
                {
                    tokens.Add(new Token("TAG_NAME", b));

                    return;
                }
                else
                {
                    b += c;
                }
            }

            tokens.Add(new Token("TAG_NAME", b));
            tokens.Add(new Token("END_TAG"));
            tokens.Add(new Token("END_DOCUMENT"));
        }

        private void parse_tag_attribute_name(string content, ref int i, List<Token> tokens)
        {
            string b = "";
            while (i < content.Length)
            {
                char c = content[i];
                i += 1;

                if (c == '>')
                {
                    tokens.Add(new Token("ATTRIBUTE_NAME", b));
                    tokens.Add(new Token("END_TAG"));

                    return;
                }
                else if (c == ' ')
                {
                    tokens.Add(new Token("ATTRIBUTE_NAME", b));

                    return;
                }
                else if (c == '=')
                {
                    tokens.Add(new Token("ATTRIBUTE_NAME", b));
                    tokens.Add(new Token("ATTRIBUTE_ASSIGNATION"));

                    return;
                }
                else
                {
                    b += c;
                }
            }

            tokens.Add(new Token("ATTRIBUTE_NAME", b));
            tokens.Add(new Token("END_TAG"));
            tokens.Add(new Token("END_DOCUMENT"));
        }

        private void parse_tag_attribute_value(string content, ref int i, List<Token> tokens)
        {
            string b = "";
            bool opened = false;
            while (i < content.Length)
            {
                char c = content[i];
                i += 1;

                if (c == '"' && !opened)
                {
                    opened = true;
                }
                else if (c == '"' && opened)
                {
                    tokens.Add(new Token("ATTRIBUTE_VALUE", b));

                    return;
                }
                else
                {
                    b += c;
                }
            }

            tokens.Add(new Token("ATTRIBUTE_VALUE", b));
            tokens.Add(new Token("END_TAG"));
            tokens.Add(new Token("END_DOCUMENT"));
        }

        private void parse_closing_tag(string content, ref int i, List<Token> tokens)
        {
            string b = "";
            while (i < content.Length)
            {
                char c = content[i];
                i += 1;

                if (c == '>')
                {
                    tokens.Add(new Token("TAG_NAME", b));
                    tokens.Add(new Token("END_CLOSING_TAG"));

                    return;
                }
                else
                {
                    b += c;
                }
            }

            tokens.Add(new Token("TAG_NAME", b));
            tokens.Add(new Token("END_CLOSING_TAG"));
            tokens.Add(new Token("END_DOCUMENT"));
        }
    }
}
