using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEnumerator
{
    Map m_map;

    public MapEnumerator(Map map)
    {
        m_map = map;
    }

    public bool IsCageType(int row, int col)
    {
        return false;
    }
}
