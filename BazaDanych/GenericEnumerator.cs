using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaDanych
{
    class GenericEnumerator : IEnumerator
    {
        private readonly object[] _list;
        // Enumerators are positioned before the first element
        // until the first MoveNext() call. 

        private int _position = -1;

        public GenericEnumerator(object[] list)
        {
            _list = list;
        }

        public bool MoveNext()
        {
            _position++;
            return (_position < _list.Length);
        }

        public void Reset()
        {
            _position = -1;
        }

        public object Current
        {
            get
            {
                try { return _list[_position]; }
                catch (IndexOutOfRangeException) { throw new InvalidOperationException(); }
            }
        }
    }
}
