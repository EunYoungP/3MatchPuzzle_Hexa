using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quest
{
    public enum MatchType
    {
        NONE        = 0,
        THREE       = 3,
        FOUR        = 4,
        FIVE        = 5,
        SIX         = 6,
        THREE_FOUR  = 7,
        THREE_FIVE  = 8,
        THREE_SIX   = 9,
        FOUR_FOUR   = 10,
        FOUR_FIVE   = 11,
        FOUR_SIX    = 12,   
        FIVE_FIVE   = 13,
        FIVE_SIX    = 14,
        SIX_SIX     = 15,
        THREE_THREE = 16
    }

    static class QuestDefine
    {
        public static short ToValue(this MatchType matchType )
        {
            return (short)matchType;
        }

        public static MatchType Add(this MatchType matchType, MatchType targetMatchType)
        {
            if (matchType == MatchType.THREE && targetMatchType == MatchType.THREE)
                return MatchType.THREE_THREE;
            else if (matchType == MatchType.FOUR && targetMatchType == MatchType.FOUR)
                return MatchType.FOUR_FOUR;

            return (MatchType)((int)matchType + (int)targetMatchType);
        }
    }
}


