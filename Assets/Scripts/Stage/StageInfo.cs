using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageInfo
{
    public int row;
    public int col;

    public Hex[,] hexs;

    // 로컬파일 정보 로드로 수정예정
    public StageInfo(int row, int col)
    {
        this.row = row;
        this.col = col;
    }
}
