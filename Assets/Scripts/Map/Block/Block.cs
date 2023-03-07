using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Base;

public class Block
{
    private BlockType blockType;
    public BlockType type { get { return blockType; } set { blockType = value; } }

    public Transform blockObj { get { return m_blockBehaviour?.transform; } }
    Vector3Int vDuplicate;

    // ¢Ø
    public int xDuplicate
    {
        get { return vDuplicate.x; }
        set { vDuplicate.x = value; }
    }

    // ¡é
    public int yDuplicate
    {
        get { return vDuplicate.y; }
        set { vDuplicate.y = value; }
    }

    // ¢×
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
}
