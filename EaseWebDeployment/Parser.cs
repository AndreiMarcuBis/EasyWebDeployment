using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace EaseWebDeployment
{
    class Parser
    {
        StringBuilder sb;
        List<Tuple<string, string>> files_to_copy;

        public void parse(List<Token> tokens)
        {
            Node root = new Node("ROOT");
            root.set_parent(root);

            Node c = root;
            string s = null;
            Node nn = null;
            bool start = true;
            foreach (Token t in tokens)
            {
                if (t.get_name() == "HTML5_DOCTYPE")
                {
                    Node nd = new Node("DOCTYPE");
                    c.add_child(nd);
                    nd.set_parent(c);
                }
                else if (t.get_name() == "START_TAG")
                {
                    start = true;
                    nn = new Node("ELEMENT");
                }
                else if (t.get_name() == "TAG_NAME")
                {
                    if (start)
                    {
                        nn.set_value(t.get_value());
                    }
                    else
                    {
                        s = t.get_value();
                    }
                }
                else if (t.get_name() == "ATTRIBUTE_NAME")
                {
                    if (t.get_value() != "")
                    {
                        nn.set_attribute(t.get_value(), null);
                    }
                    s = t.get_value();
                }
                else if (t.get_name() == "ATTRIBUTE_VALUE")
                {
                    if (s != "")
                    {
                        nn.set_attribute(s, t.get_value());
                    }
                }
                else if (t.get_name() == "END_TAG")
                {
                    c.add_child(nn);
                    nn.set_parent(c);

                    if (nn.get_value() == "link" || nn.get_value() == "meta" || nn.get_value() == "path")
                    {

                    }
                    else
                    {
                        c = nn;
                    }
                }
                else if (t.get_name() == "CONTENT")
                {
                    Node nc = new Node("CONTENT", t.get_value());
                    c.add_child(nc);
                    nc.set_parent(c);
                }
                else if (t.get_name() == "START_CLOSING_TAG")
                {
                    start = false;
                }
                else if (t.get_name() == "END_CLOSING_TAG")
                {
                    Node p = c;

                    while (p.get_value() != s && p != root)
                    {
                        p = p.get_parent();
                    }

                    c = p.get_parent();
                }
            }

            files_to_copy = new List<Tuple<string, string>>();
            sb = new StringBuilder();
            root.browse(prefix, suffix);

            File.WriteAllText(@"test.html", sb.ToString());

            Console.ReadKey();
        }

        private void prefix(Node n, int i)
        {
            if (n.get_type() == "DOCTYPE")
            {
                sb.Append("<!DOCTYPE html>\r\n");
            }
            else if (n.get_type() == "ELEMENT")
            {
                sb.Append("<" + n.get_value());

                List<KeyValuePair<string, string>> at = n.get_attributes();
                Dictionary<string, string> d = new Dictionary<string, string>();
                foreach (KeyValuePair<string, string> p in at)
                {
                    d[p.Key] = p.Value;
                }

                if (d.ContainsKey("data-compilation-set-attribute"))
                {
                    string a = d["data-compilation-set-attribute"];
                    d.Remove("data-compilation-set-attribute");

                    string[] parts = a.Split(new char[] { ':' }, 2);

                    string attr = parts[0];
                    string val = parts[1];

                    if (d.ContainsKey("data-compilation-copy-file") && d.ContainsKey(attr))
                    {
                        string oval = d[attr];

                        files_to_copy.Add(new Tuple<string, string>(oval, val));
                    }

                    d[attr] = val;
                }


                foreach (KeyValuePair<string, string> p in d)
                {
                    sb.Append(" " + p.Key);
                    if (p.Value != null)
                    {
                        sb.Append("=\"" + p.Value + "\"");
                    }
                }

                sb.Append(">");
            }
            else if (n.get_type() == "CONTENT")
            {
                sb.Append(n.get_value());
            }
        }

        private void suffix(Node n, int i)
        {
            if (n.get_type() == "ELEMENT")
            {
                if (n.get_value() == "link" || n.get_value() == "meta" || n.get_value() == "path" || n.get_value() == "br" || n.get_value() == "img" || n.get_value() == "input")
                {

                }
                else
                {
                    sb.Append("</" + n.get_value() + ">");
                }
            }
        }
    }
}