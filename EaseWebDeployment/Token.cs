using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EaseWebDeployment
{
    class Token
    {
        string name;
        string value;

        public Token(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        public Token(string name)
        {
            this.name = name;
            value = "";
        }

        public string get_name()
        {
            return name;
        }

        public string get_value()
        {
            return value;
        }
    }
}
