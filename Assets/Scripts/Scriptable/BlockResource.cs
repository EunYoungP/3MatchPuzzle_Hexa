using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Block", fileName ="BlockResource.asset")]
    public class BlockResource : ScriptableObject
    {
        public Sprite[] basicBlocksprites;
    }
}
