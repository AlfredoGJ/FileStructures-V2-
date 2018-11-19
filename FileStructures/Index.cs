using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStructures
{
    class Index
    {
        public long pos;
        public char type;
        public int lenght;
        public int slots;
        public long next;

        // Index Size=8+1+4+4+8= 25


        public Index(long p, char t, int l, int s)
        {
            pos = p;
            type = t;
            lenght = l;
            slots = s;
            next = -1;
        }
        
    }
}
