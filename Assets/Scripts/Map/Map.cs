using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MapShuffle;

public class Map : MonoBehaviour
{
    private int m_width;
    private int m_height;

    public int width { get { return m_width; } }
    public int height { get { return m_height; } }

    private Hex[,] m_hexs;
    public Hex[,] hexs { get { return m_hexs; } }
    private Block[,] m_blocks;
    public Block[,] blocks { get { return m_blocks; } }

    GameObject hexPrefab;
    GameObject blockPrefab;
    Transform container;

    public Map(int width, int height)
    {
        this.m_width = width;
        this.m_height = height;

        // Hex, Block 생성자 구현 예정
        m_hexs = new Hex[width, height];
        m_blocks = new Block[width, height];
    }

    private void MakeHexaMap()
    {
        // 육각형 맵 피봇
        Vector2Int bottomPivot = new Vector2Int(m_width / 2, 0);
        Vector2Int topPivot = new Vector2Int(m_width / 2, m_height - 1);

        // 왼쪽 이동 피봇
        Vector2Int LeftBottomPivot = bottomPivot;
        Vector2Int LeftTopPivot = topPivot;

        // 오른쪽 이동 피봇
        Vector2Int RightBottomPivot = bottomPivot;
        Vector2Int RightTopPivot = topPivot;

        // 1. 육각형 왼쪽 처리
        for (int x = m_width / 2; x > 0; x--)
        {
            if (x % 2 == 1)
            {
                LeftBottomPivot.x += HexDirection.oddDirectionOffset[2].x;
                LeftBottomPivot.y += HexDirection.oddDirectionOffset[2].y;

                LeftTopPivot.x += HexDirection.oddDirectionOffset[1].x;
                LeftTopPivot.y += HexDirection.oddDirectionOffset[1].y;
            }
            else
            {
                LeftBottomPivot.x += HexDirection.evenDirectionOffset[2].x;
                LeftBottomPivot.y += HexDirection.evenDirectionOffset[2].y;

                LeftTopPivot.x += HexDirection.evenDirectionOffset[1].x;
                LeftTopPivot.y += HexDirection.evenDirectionOffset[1].y;
            }

            // 1-1. 왼쪽 아래 
            for (int y = LeftBottomPivot.y - 1; y >= 0; y--)
            {
                m_hexs[LeftBottomPivot.x, y].type = HexType.EMPTY;
            }

            // 1-2. 왼쪽 위
            for (int y = LeftTopPivot.y + 1; y < m_height; y++)
            {
                m_hexs[LeftTopPivot.x, y].type = HexType.EMPTY;
            }
        }

        // 2. 육각형 오른쪽 처리
        for (int x = m_width / 2; x < m_width-1; x++)
        {
            if (x % 2 == 1)
            {
                RightBottomPivot.x += HexDirection.oddDirectionOffset[4].x;
                RightBottomPivot.y += HexDirection.oddDirectionOffset[4].y;

                RightTopPivot.x += HexDirection.oddDirectionOffset[5].x;
                RightTopPivot.y += HexDirection.oddDirectionOffset[5].y;
            }
            else
            {
                RightBottomPivot.x += HexDirection.evenDirectionOffset[4].x;
                RightBottomPivot.y += HexDirection.evenDirectionOffset[4].y;

                RightTopPivot.x += HexDirection.evenDirectionOffset[5].x;
                RightTopPivot.y += HexDirection.evenDirectionOffset[5].y;
            }

            // 1-1. 오른쪽 아래
            for (int y = RightBottomPivot.y - 1; y >= 0; y--)
            {
                m_hexs[RightBottomPivot.x, y].type = HexType.EMPTY;
            }

            // 1-2. 오른쪽 위
            for (int y = RightTopPivot.y + 1; y < m_height; y++)
            {
                m_hexs[RightTopPivot.x, y].type = HexType.EMPTY;
            }
        }
    }

    internal void ComposeMap(GameObject hexPrefab, GameObject blockPrefab, Transform container)
    {
        MakeHexaMap();

        this.hexPrefab = hexPrefab;
        this.blockPrefab = blockPrefab;
        this.container = container;

        MapShuffler mapShuffler = new MapShuffler(this, true);
        mapShuffler.Shuffle();

        for (int x = 0; x < m_width; x++)
        {
            for (int y = 0; y < m_height; y++)
            {
                if (m_hexs[x, y].type == HexType.EMPTY)
                {
                    m_blocks[x, y].type = BlockType.EMPTY;
                    continue;
                }

                Hex hex = m_hexs[x, y].InstantiateHex(hexPrefab, container);
                hex.Move(x, y);
                hex.SetObjectName(x, y);

                Block block = m_blocks[x, y].InstantiateBlock(blockPrefab, container);
                block.Move(x, y);
                block.SetObjectName(x, y);
            }
        }
    }

    public bool CanShuffle(int row, int col, bool isLoading)
    {
        if (!m_hexs[row, col].type.isBlockMovableType())
            return false;

        return true;
    }

    public void ChangeBlock(Block block, BlockVariety notAllowedVariety)
    {
        BlockVariety newBlockVariety;

        while (true)
        {
            newBlockVariety = (BlockVariety)Random.Range(0, 6);

            if (newBlockVariety == notAllowedVariety)
                continue;

            break;
        }
        block.blockVariety = newBlockVariety;
    }

    public bool IsSwipeable(int row, int col)
    {
        return m_hexs[row, col].type.isBlockMovableType();
    }
}
