using System;

namespace System
{
    public class Tuple<T1, T2>
    {
        public T1 Type1
        {
            get; private set;
        }

        public T2 Type2
        {
            get; private set;
        }

        public Tuple(T1 type1, T2 type2)
        {
            Type1 = type1;
            Type2 = type2;
        }
    }
}
