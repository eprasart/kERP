using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kERP.Common
{
    class Identity
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public Identity(int id, string text)
        {
            Id = id;
            Text = text;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
