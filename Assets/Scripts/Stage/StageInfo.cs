using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageInfo
{
    public int row;
    public int col;

    public Hex[,] hexs;

    // �������� ���� �ε�� ��������
    public StageInfo(int row, int col)
    {
        this.row = row;
        this.col = col;
    }
}
