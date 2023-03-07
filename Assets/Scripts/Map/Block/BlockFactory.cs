using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BlockFactory
{
    public static Block SpawnBlock(BlockType blockType)
    {
        Block block = new Block(blockType);

        if(blockType ==BlockType.EMPTY)
        {
            block.blockVariety = BlockVariety.NULL;
        }
        else if(blockType == BlockType.BASIC)
        {
            block.blockVariety = (BlockVariety)Random.Range(0, 6);
        }

        return block;
    }
}
