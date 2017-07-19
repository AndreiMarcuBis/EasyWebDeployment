using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EaseWebDeployment
{
    class Node
    {
        Node parent;
        List<Node> children;
        Dictionary<string, string> attributes;

        string value;
        string type;

        public Node(string type, string value)
        {
            attributes = new Dictionary<string, string>();
            children = new List<Node>();
            this.type = type;
            this.value = value;
        }

        public Node(string type)
        {
            attributes = new Dictionary<string, string>();
            children = new List<Node>();
            this.type = type;
            value = "";
        }

        public List<KeyValuePair<string, string>> get_attributes()
        {
            return attributes.ToList();
        }

        public void add_child(Node n)
        {
            children.Add(n);
        }

        public string get_value()
        {
            return value;
        }

        public void set_value(string value)
        {
            this.value = value;
        }

        public void set_attribute(string name, string value)
        {
            attributes[name] = value;
        }

        public void set_parent(Node parent)
        {
            this.parent = parent;
        }

        public Node get_parent()
        {
            return parent;
        }

        public void browse(Action<Node, int> p, Action<Node, int> s)
        {
            browse(p, s, 0);
        }

        private void browse(Action<Node, int> p, Action<Node, int> s, int i)
        {
            p(this, i);

            foreach (Node c in children)
            {
                c.browse(p, s, i + 1);
            }

            s(this, i);
        }

        public string get_type()
        {
            return type;
        }
    }
}
