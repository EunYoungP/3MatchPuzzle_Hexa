using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public enum SwipeType
    {
        NULL        = -1,
        UP_RIGHT    = 0,
        UP          = 1,
        UP_LEFT     = 2,
        DOWN_LEFT   = 3,
        DOWN        = 4,
        DOWN_RIGHT  = 5
    }

    public static class TouchEvaluator
    {
        public static SwipeType EvalSwipeDir(Vector2 start, Vector2 end)
        {
            float angle = EvalDragAngle(start, end);

            if (angle < 0)
                return SwipeType.NULL;

            int swipe = (((int)angle) % 360) / 60;

            switch(swipe)
            {
                case 0: return SwipeType.UP_RIGHT;
                case 1: return SwipeType.UP;      
                case 2: return SwipeType.UP_LEFT;
                case 3: return SwipeType.DOWN_LEFT;
                case 4: return SwipeType.DOWN;
                case 5: return SwipeType.DOWN_RIGHT;
            }
            return SwipeType.NULL;
        }

        private static float EvalDragAngle(Vector2 start, Vector2 end)
        {
            Vector2 dragDir = end - start;

            if (dragDir.magnitude <= 0.2f)
                return -1f;

            float aimAngle = Mathf.Atan2(dragDir.y, dragDir.x);
            if(aimAngle < 0f)
            {
                aimAngle = Mathf.PI * 2 + aimAngle;
            }
            return aimAngle * Mathf.Rad2Deg;
        }
    }

    public static class SwipeDirMethod
    {
        public static int GetTargetRow(this SwipeType swipeDir, int row)
        {
            switch (swipeDir)
            {
                case SwipeType.DOWN_LEFT: return -1;
                case SwipeType.UP_LEFT: return -1;
                case SwipeType.UP_RIGHT: return 1;
                case SwipeType.DOWN_RIGHT: return 1;
                default:
                    return 0;
            }
        }

        public static int GetTargetCol(this SwipeType swipeDir, int row)
        {
            bool isOdd = ((row % 2) == 1);

            if(isOdd)
            {
                switch (swipeDir)
                {
                    case SwipeType.DOWN: return -1;
                    case SwipeType.UP_LEFT: return 1;
                    case SwipeType.UP: return 1;
                    case SwipeType.UP_RIGHT: return 1;
                    default:
                        return 0;
                }
            }
            else
            {
                switch (swipeDir)
                {
                    case SwipeType.DOWN: return -1;
                    case SwipeType.DOWN_LEFT: return -1;
                    case SwipeType.UP: return 1;
                    case SwipeType.DOWN_RIGHT: return -1;
                }
            }
            return 0;
        }
    }
}
