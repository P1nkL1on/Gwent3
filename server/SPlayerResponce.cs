using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class SPlayerResponse
    {
        public SPlayerResponse() { _value = -1; }
        public SPlayerResponse(int value) { _value = value; }
        public int value { get { return _value; } }
        int _value;
    }
}
