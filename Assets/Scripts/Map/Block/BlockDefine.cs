using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BlockType
{
    EMPTY,
    BASIC,
}

public enum BlockVariety
{
    NULL = -1,
    VARIETY_0 = 0,
    VARIETY_1 = 1,
    VARIETY_2 = 2,
    VARIETY_3 = 3,
    VARIETY_4 = 4,
    VARIETY_5 = 5,
}

public static class BlockMethod
{
    public static bool IsSafeEqual(this Block block, Block targetBlock)
    {
        if (block == null)
            return false;

        return (block.IsEqual(targetBlock));
    }
}
