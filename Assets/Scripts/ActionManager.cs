using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class ActionManager
{
    private Transform m_MapTransform;
    private Stage m_Stage;
    private MonoBehaviour m_MonoBehaviour;
    private bool isRunning;

    public ActionManager(Transform mapTransform, Stage stage)
    {
        m_MapTransform = mapTransform;
        m_Stage = stage;

        m_MonoBehaviour = mapTransform.parent.gameObject.GetComponent<MonoBehaviour>();
    }

    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return m_MonoBehaviour.StartCoroutine(routine);
    }

    public void DoSwipeAction(int row, int col, SwipeType swipeDir)
    {
        if (m_Stage.IsValidSwpie(row, col, swipeDir))
            StartCoroutine(CoDoSwipeAction(row, col, swipeDir));
    }

    IEnumerator CoDoSwipeAction(int row, int col, SwipeType swipeDir)
    {
        if(!isRunning)
        {
            isRunning = true;

            // �� �������� ��� ���� ��ü
            Returnable<bool> swipedBlock = new Returnable<bool>(false);
            yield return m_Stage.CoDoSwipeAction(row, col, swipeDir, swipedBlock);

            if(swipedBlock.value)
            {
                // �� ��ġ �Ǵ� ��� ���� ��ü
                Returnable<bool> evalBlock = new Returnable<bool>(false);
                yield return EvaluateMap(evalBlock);

                if (!evalBlock.value)
                {
                    yield return CoDoSwipeAction(row, col, swipeDir);
                }
            }
            isRunning = false;
        }
        yield break;
    }

    IEnumerator EvaluateMap(Returnable<bool> matchResult)
    {
        yield return m_Stage.Evaluate(matchResult);
    }

}
