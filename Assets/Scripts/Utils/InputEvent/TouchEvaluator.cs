using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public enum Swipe
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
        public static Swipe EvalSwipeDir(Vector2 start, Vector2 end)
        {
            float angle = EvalDragAngle(start, end);

            if (angle < 0)
                return Swipe.NULL;

            int swipe = (((int)angle) % 360) / 60;

            switch(swipe)
            {
                case 0: return Swipe.UP_RIGHT;
                case 1: return Swipe.UP;      
                case 2: return Swipe.UP_LEFT;
                case 3: return Swipe.DOWN_LEFT;
                case 4: return Swipe.DOWN;
                case 5: return Swipe.DOWN_RIGHT;
            }
            return Swipe.NULL;
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
        public static int GetTargetRow(this Swipe swipeDir, int row)
        {
            switch (swipeDir)
            {
                case Swipe.DOWN_LEFT: return -1;
                case Swipe.UP_LEFT: return -1;
                case Swipe.UP_RIGHT: return 1;
                case Swipe.DOWN_RIGHT: return 1;
                default:
                    return 0;
            }
        }

        public static int GetTargetCol(this Swipe swipeDir, int row)
        {
            bool isOdd = ((row % 2) == 1);

            if(isOdd)
            {
                switch (swipeDir)
                {
                    case Swipe.DOWN: return -1;
                    case Swipe.UP_LEFT: return 1;
                    case Swipe.UP: return 1;
                    case Swipe.UP_RIGHT: return 1;
                    default:
                        return 0;
                }
            }
            else
            {
                switch (swipeDir)
                {
                    case Swipe.DOWN: return -1;
                    case Swipe.DOWN_LEFT: return -1;
                    case Swipe.UP: return 1;
                    case Swipe.DOWN_RIGHT: return -1;
                }
            }
            return 0;
        }
    }
}
