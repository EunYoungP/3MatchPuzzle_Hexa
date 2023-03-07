using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public class InputManager
    {
        Transform m_MapTransform;

#if UNITY_ANDROID && !UNITY_EDITOR
        IInputHandlerBase m_InputHandler = new TouchHandler();
#else
        IInputHandlerBase m_InputHandler = new MouseHandler();
#endif
        public InputManager(Transform mapTR)
        {
            m_MapTransform = mapTR;
        }

        public bool isInputDown => m_InputHandler.isInputDown;
        public bool isInputUp => m_InputHandler.isInputUp;
        public Vector2 touchPosition => m_InputHandler.inputPosition;
        public Vector2 touchToMapPosition => TouchToPosition(m_InputHandler.inputPosition);

        private Vector2 TouchToPosition(Vector3 InputPosition)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(InputPosition);

            Vector3 mapLocalPos = m_MapTransform.InverseTransformPoint(worldPos);

            return mapLocalPos;
        }

        public Swipe EvalSwipeDir(Vector2 start, Vector2 end)
        {
            return TouchEvaluator.EvalSwipeDir(start, end);
        }
    }
}
