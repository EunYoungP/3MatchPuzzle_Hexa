using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HexType
{
    EMPTY,
    BASIC,
}

public class HexDirection
{
    public static List<Vector2Int> oddDirectionOffset = new List<Vector2Int>
    {
        new Vector2Int(0,-1),  // 0, bottom
        new Vector2Int(-1,0),  // 1, bottom left
        new Vector2Int(-1,1),  // 2, top left
        new Vector2Int(0,1),   // 3, top
        new Vector2Int(1,1),   // 4, top right
        new Vector2Int(1,0)    // 5, bottom right
    };

    public static List<Vector2Int> evenDirectionOffset = new List<Vector2Int>
    {
        new Vector2Int(0,-1),  // 0, bottom
        new Vector2Int(-1,-1), // 1, bottom left
        new Vector2Int(-1,0),  // 2, top left
        new Vector2Int(0,1),   // 3, top
        new Vector2Int(1,0),   // 4, top right
        new Vector2Int(1,-1)   // 5, bottom right
    };
}

public static class HexTypeMethod
{
    public static bool isBlockAllocatableType(this HexType hexType)
    {
        return !(hexType == HexType.EMPTY);
    }

    public static bool isBlockMovableType(this HexType hexType)
    {
        return !(hexType == HexType.EMPTY);
    }
}

