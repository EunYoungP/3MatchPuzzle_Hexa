using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class StageController : MonoBehaviour
{
    [SerializeField] GameObject hexPrefab;
    [SerializeField] GameObject blockPrefab;
    [SerializeField] Transform mapTransform;

    private bool isInit;
    private Stage m_stage;
    private InputManager m_InputManager;
    private ActionManager m_ActionManager;

    private bool m_TouchAvailable;
    private Vector2Int m_BlockIndex;
    private Vector3 m_ClickPos;

    private void Start()
    {
        InitStage();
    }

    private void InitStage()
    {
        if (isInit)
            return;

        isInit = true;
        m_InputManager = new InputManager(mapTransform);

        BuildStage();
    }

    private void Update()
    {
        OnInputHandler();
    }

    private void BuildStage()
    {
        m_stage = StageBuilder.BuildStage(21);
        m_ActionManager = new ActionManager(mapTransform, m_stage);

        m_stage.ComposeStage(hexPrefab, blockPrefab, mapTransform);
    }

    private void OnInputHandler()
    {
        // 마우스 버튼 DOWN
        if (!m_TouchAvailable && m_InputManager.isInputDown)
        {
            Vector2 localPoint = m_InputManager.touchToMapPosition;

            if (!m_stage.IsInsideMap(localPoint))
                return;

            Vector2Int blockIdx;
            if (m_stage.IsOnVailedBlock(localPoint, out blockIdx))
            {
                m_TouchAvailable = true;
                m_BlockIndex = blockIdx;
                m_ClickPos = localPoint;
            }
        }
        // 마우스 버튼 UP
        else if (m_TouchAvailable && m_InputManager.isInputUp)
        {
            Vector2 localPoint = m_InputManager.touchToMapPosition;

            SwipeType swipeDir = m_InputManager.EvalSwipeDir(m_ClickPos, localPoint);

            Debug.Log($"Swipe : {swipeDir}, Block = {m_BlockIndex}");

            if (swipeDir != SwipeType.NULL)
            {
                m_ActionManager.DoSwipeAction(m_BlockIndex.x, m_BlockIndex.y, swipeDir);
            }

            m_TouchAvailable = false;
        }
    }
}
