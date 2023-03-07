using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public class Returnable<T>
    {
        public T value { get; set; }

        public Returnable(T t)
        {
            value = t;
        }
    }
}
