using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Base;
using Util;

public class Stage
{
    private Map m_map;
    public Map map { get { return m_map; } }

    private StageBuilder stageBuilder;

    public Hex[,] hexs { get { return m_map.hexs; } }
    public Block[,] blocks { get { return m_map.blocks; } }

    public Stage(StageBuilder stageBuilder, StageInfo stageInfo)
    {
        this.stageBuilder = stageBuilder;
        m_map = new Map(stageInfo.row, stageInfo.col);
    }

    internal void ComposeStage(GameObject m_hexPrefab, GameObject blockPrefab, Transform parent)
    {
        m_map.ComposeMap(m_hexPrefab, blockPrefab, parent);
    }

    public bool IsInsideMap(Vector2 blockIdx)
    {
        // temporary
        return true;
    }

    // localPoint : local Point when mouse down
    public bool IsOnVailedBlock(Vector2 localPoint, out Vector2Int blockIndex)
    {
        Debug.Log($"localPoint : {localPoint}");
        float localToBlock_X = (localPoint.x - Constant.TO_CENTER_OFFSET.x) / Constant.XOFFSET;
        int row = (int)(localToBlock_X + 0.5f);

        float localToBlock_Y = localPoint.y;

        if (row % 2 ==1)
        {
            localToBlock_Y = localToBlock_Y - Constant.TO_CENTER_OFFSET.y;
            localToBlock_Y -= (Constant.YOFFSET / 2);
            localToBlock_Y /= Constant.YOFFSET;
        }
        else
        {
            localToBlock_Y = (localToBlock_Y - Constant.TO_CENTER_OFFSET.y) / Constant.YOFFSET;
        }
        
        int col = (int)(localToBlock_Y + 0.5f);
        blockIndex = new Vector2Int(row, col);
        Debug.Log($"mouse down block index : {blockIndex}");

        return m_map.IsSwipeable(row, col);
    }

    public bool IsValidSwpie(int row, int col, SwipeType swipeDir)
    {
        switch (swipeDir)
        {
            case SwipeType.UP: return col < m_map.height -1;
            case SwipeType.DOWN: return col > 0;
            case SwipeType.UP_RIGHT:
                {
                    if (row % 2 == 1)
                        return col < map.height - 1 && row < map.width - 1;
                    else
                        return row < map.width - 1;
                }
            case SwipeType.UP_LEFT:
                {
                    if (row % 2 == 1)
                        return row > 0 && col < map.height - 1;
                    else
                        return row > 0;
                }
            case SwipeType.DOWN_LEFT:
                {
                    if (row % 2 == 1)
                        return row > 0;
                    else
                        return row > 0;
                }
            case SwipeType.DOWN_RIGHT:
                {
                    if (row % 2 == 1)
                        return row < map.width - 1;
                    else
                        return row < map.width - 1;
                }
            default: return false;
        }
    }

    public IEnumerator CoDoSwipeAction(int row, int col, SwipeType swipeDir, Returnable<bool> actionResult)
    {
        actionResult.value = false;

        int swipeRow = row;
        int swipeCol = col;
        swipeRow += swipeDir.GetTargetRow(row);
        swipeCol += swipeDir.GetTargetCol(row);

        if(m_map.IsSwipeable(swipeRow, swipeCol))
        {
            Block baseBlock = blocks[row, col];
            Block swipeBlock = blocks[swipeRow, swipeCol];
            Vector3 basePos = baseBlock.blockObj.transform.position;
            Vector3 swipePos = swipeBlock.blockObj.transform.position;

            if(swipeBlock.IsSwipeable(baseBlock))
            {
                baseBlock.MoveTo(swipePos, Constant.SWIPE_DURATION);
                swipeBlock.MoveTo(basePos, Constant.SWIPE_DURATION);

                yield return new WaitForSeconds(Constant.SWIPE_DURATION);

                blocks[row, col] = swipeBlock;
                blocks[swipeRow, swipeCol] = baseBlock;

                actionResult.value = true;
            }
        }
        yield break;
    }

    public IEnumerator Evaluate(Returnable<bool> matchResult)
    {
        yield return m_map.Evaluate(matchResult);
    }
}
