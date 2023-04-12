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

public enum BlockStatus
{
    NORMAL,
    MATCH,
    CLEAR
}

public enum BlockQuestType
{
    NONE = -1,
    CLEAR_SIMPLE = 0,
    CLEAR_VERT = 1,
    CLEAR_UP_DIAGONAL = 2,
    CLEAR_DOWN_DIAGONAL = 3,
    CLEAR_CIRCLE = 4,               // 주변 블럭 제거
    CLEAR_LAZER = 5,                // 지정 블럭과 동일한 블럭 모두 제거
    CLEAR_VERT_BUFF = 6,            // VERTICAL + CIRCLE
    CLEAR_UP_DIAGONAL_BUFF = 7,     // UP_DIAGONAL + CIRCLE
    CLEAR_DOWN_DIAGONAL_BUFF = 8,   // DOWN_DIAGONAL + CIRCLE
    CLEAR_CIRCLE_BUFF = 9,          // CIRCLE + CIRCLE
    CLEAR_LAZER_BUFF = 10           // LAZER + LAZER
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
