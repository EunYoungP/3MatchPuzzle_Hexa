using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Base;
using Quest;

public class Block
{
    private BlockType blockType;
    public BlockType type { get { return blockType; } set { blockType = value; } }

    public Transform blockObj { get { return m_blockBehaviour?.transform; } }
    Vector3Int vDuplicate;

    // 블럭 매칭
    public BlockStatus blockStatus;
    public BlockQuestType blockQuestType;
    public MatchType matchType;
    public short matchCount;

    // 블럭 내구도, 클리어 되기위한 매치 횟수
    int durability;
    public virtual int Durability
    {
        get { return durability; }
        set { durability = value; }
    }

    // ↖
    public int xDuplicate
    {
        get { return vDuplicate.x; }
        set { vDuplicate.x = value; }
    }

    // ↓
    public int yDuplicate
    {
        get { return vDuplicate.y; }
        set { vDuplicate.y = value; }
    }

    // ↙
    public int zDuplicate
    {
        get { return vDuplicate.z; }
        set { vDuplicate.z = value; }
    }

    public void RestDuplicationInfo()
    {
        vDuplicate.x = 0;
        vDuplicate.y = 0;
        vDuplicate.z = 0;
    }

    private BlockVariety m_blockVariety;
    public BlockVariety blockVariety
    {
        get
        {
            return m_blockVariety;
        }
        set
        {
            m_blockVariety = value;
            m_blockBehaviour?.UpdateBlockImage();
        }
    }

    private BlockBehaviour m_blockBehaviour;
    public BlockBehaviour blockBehaviour
    {
        get
        {
            return m_blockBehaviour;
        }
        set
        {
            m_blockBehaviour = value;
            m_blockBehaviour.SetBlock(this);
        }
    }

    public Block(BlockType blockType)
    {
        type = blockType;

        blockStatus = BlockStatus.NORMAL;
        blockQuestType = BlockQuestType.CLEAR_SIMPLE;
        matchType = MatchType.NONE;
        durability = 1;
    }

    public Block InstantiateBlock(GameObject blockPrefab, Transform container)
    {
        GameObject block = Object.Instantiate(blockPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        block.transform.parent = container;
        blockBehaviour = block.GetComponent<BlockBehaviour>();

        return this;
    }

    public void Move(float x, float y)
    {
        float yPos = y * Constant.YOFFSET;

        if (x % 2 == 1)
            yPos += Constant.YOFFSET / 2;

        blockBehaviour.transform.position = new Vector3(x * Constant.XOFFSET, yPos, 0) + Constant.TO_CENTER_OFFSET + Constant.BLOCK_OFFSET;
    }

    public void SetObjectName(int x, int y)
    {
        blockBehaviour.name = "Block" + x + "_" + y;
    }

    public bool IsEqual(Block target)
    {
        if (IsMatchableBlock() && this.blockVariety == target.blockVariety)
            return true;
        return false;
    }

    public bool IsMatchableBlock()
    {
        return !(type == BlockType.EMPTY);
    }

    public bool IsSwipeable(Block baseBlock)
    {
        // Temporary
        return true;
    }

    public void MoveTo(Vector3 to, float duration)
    {
        m_blockBehaviour.StartCoroutine(Util.Action2D.MoveTo(blockObj, to, duration));
    }

    public void UpdateBlcokStatusMatch(MatchType newMatchType, bool accumulate = true)
    {
        this.blockStatus = BlockStatus.MATCH;

        if (matchType == MatchType.NONE)
            this.matchType = newMatchType;
        else
        {
            this.matchType = accumulate ? matchType.Add(newMatchType) : newMatchType;
        }
        matchCount = matchType.ToValue();
    }

    public bool DoEvaluation(MapEnumerator mapEnumerator, int row, int col)
    {
        Debug.Assert(mapEnumerator != null, $"({row}, {col})");

        if (!IsEvaluatable())
            return false;

        // 1. 매치된 상태의 블럭
        if (blockStatus == BlockStatus.MATCH)
        {
            if (blockQuestType == BlockQuestType.CLEAR_SIMPLE || mapEnumerator.IsCageType(row, col))
            {
                Debug.Assert(durability > 0, "durability is zero");

                Durability--;
            }
        }
        else
        {
            return true;
        }

        if(Durability == 0)
        {
            blockStatus = BlockStatus.CLEAR;
            return false;
        }

        // 클리어 조건에 도달하지 않은 경우 기본 상태로 변경
        blockStatus = BlockStatus.NORMAL;
        matchType = MatchType.NONE;
        matchCount = 0;

        return false;
    }

    public bool IsEvaluatable()
    {
        if (blockStatus == BlockStatus.CLEAR || !IsMatchableBlock())
            return false;

        return true;
    }

    public virtual void Destroy()
    {
        Debug.Assert(blockObj != null);
        blockBehaviour.OnActionClear();
    }
}
