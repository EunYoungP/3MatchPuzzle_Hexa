using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBuilder
{
    private int curStage;

    public StageBuilder(int stageIdx)
    {
        curStage = stageIdx;
    }

    public static Stage BuildStage(int stageIdx)
    {
        StageBuilder stageBuilder = new StageBuilder(stageIdx);
        Stage stage = stageBuilder.ComposeStage();
        return stage;
    }

    public Stage ComposeStage()
    {
        // 임시 스테이지 정보
        StageInfo stageInfo = new StageInfo(7, 6);

        Stage stage = new Stage(this, stageInfo);

        for(int i = 0; i< stageInfo.row; i++)
        {
            for( int j = 0; j < stageInfo.col; j++)
            {
                stage.hexs[i, j] = SpawnHex();
                stage.blocks[i, j] = SpawnBlocks();
            }
        }
        return stage ;
    }

    private Hex SpawnHex()
    {
        return new Hex(HexType.BASIC);
    }

    private Block SpawnBlocks()
    {
        return BlockFactory.SpawnBlock(BlockType.BASIC);
    }
}
