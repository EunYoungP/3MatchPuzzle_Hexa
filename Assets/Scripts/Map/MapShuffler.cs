using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapShuffle
{
    using BlockVector = KeyValuePair<Block, Vector2Int>;

    public class MapShuffler : MonoBehaviour
    {
        Map map;
        bool isLoadingShuffle;

        SortedList<int, BlockVector> randomizeBlocks = new SortedList<int, BlockVector>();
        Queue<BlockVector> unUsedBlocks = new Queue<BlockVector>();
        IEnumerator<KeyValuePair<int, BlockVector>> it;
        bool isListComplete;

        public MapShuffler(Map map, bool isInit)
        {
            this.map = map;
            isLoadingShuffle = isInit;
        }

        public void Shuffle()
        {
            // �� ���� ��Ī ������ ������Ʈ�մϴ�.
            PrepareDuplicationDatas();

            // ���� ��� ���� ����Ʈ�� �����մϴ�.
            PrepareShuffleBlocks();

            // �غ��� �����͸� �̿��� ������ �����մϴ�.
            RunShuffle();
        }

        private void PrepareDuplicationDatas()
        {
            for(int row = 0; row < map.width; row++)
            {
                for(int col = 0; col < map.height; col++)
                {
                    Block block = map.blocks[row, col];

                    if (block == null)
                        continue;

                    if (map.CanShuffle(row, col, isLoadingShuffle))
                        block.RestDuplicationInfo();
                    else
                    {
                        block.zDuplicate = 1;
                        block.xDuplicate = 1;
                        block.yDuplicate = 1;

                        /*
                        if(col > 0 && !map.CanShuffle(row, col-1,isLoadingShuffle) && map.blocks[row,col-1].IsSafeEqual(block))
                        {
                            block.yDuplicate = 2;
                            map.blocks[row, col - 1].yDuplicate = 2;
                        }

                        if (row > 0 && !map.CanShuffle(row, col - 1, isLoadingShuffle) && map.blocks[row, col - 1].IsSafeEqual(block))
                        {
                            block.yDuplicate = 2;
                            map.blocks[row, col - 1].yDuplicate = 2;
                        }

                        if (col > 0 && !map.CanShuffle(row, col - 1, isLoadingShuffle) && map.blocks[row, col - 1].IsSafeEqual(block))
                        {
                            block.yDuplicate = 2;
                            map.blocks[row, col - 1].yDuplicate = 2;
                        }
                        */
                    }   // �������� �ʴ� �� ó��
                }
            }
        }

        private void PrepareShuffleBlocks()
        {
            for(int row = 0; row < map.width; row++)
            {
                for(int col = 0; col < map.height; col++)
                {
                    if (!map.CanShuffle(row, col, isLoadingShuffle))
                        continue;

                    while(true)
                    {
                        int rndKey = Random.Range(0, 10000);

                        if (randomizeBlocks.ContainsKey(rndKey))
                            continue;

                        randomizeBlocks.Add(rndKey, new BlockVector(map.blocks[row, col], new Vector2Int(row, col)));
                        break;
                    }
                }
            }
            it = randomizeBlocks.GetEnumerator();
        }

        private void RunShuffle()
        {
            for(int row = 0; row < map.width; row++)
            {
                for(int col = 0; col < map.height; col++)
                {
                    if (!map.CanShuffle(row, col, isLoadingShuffle))
                        continue;

                    map.blocks[row, col] = GetShuffleBlock(row, col);
                }
            }
        }

        private Block GetShuffleBlock(int row, int col)
        {
            BlockVariety prevVariety = BlockVariety.NULL;
            Block firstBlock = null;
            bool useQueue = true;   

            while(true)
            {
                // 1. Queue���� ���� �ϳ� �����ϴ�. ù��° �ĺ�
                BlockVector blockInfo = NextBlock(useQueue);
                Block block = blockInfo.Key;

                // 2. ����Ʈ���� ���� ��� ó���� ���. : ��ü �������� 1ȸ �߻�
                if(block == null)
                {
                    blockInfo = NextBlock(true);
                    block = blockInfo.Key;
                }

                if (prevVariety == BlockVariety.NULL)
                    prevVariety = block.blockVariety;

                // 3. ����Ʈ�� ��� ó�� �� ���
                if(isListComplete)
                {
                    if(firstBlock == null)
                    {
                        // 3-1. ����Ʈ�� ó���ϰ�, ó������ ť���� ���� ���
                        firstBlock = block;
                    }
                    else if( ReferenceEquals(firstBlock, block))
                    {
                        // 3-2. ť�� ��� ���� ���ǿ� ���� �ʾ� ù��° ��ҷ� ���ƿ� ���
                        map.ChangeBlock(block, prevVariety);
                    }
                }

                // 4. ���� ���� ���� ���� ���ӵǴ� ������ ���
                Vector3Int vtDup = CalcDuplications(row, col, block);

                // 5. 2�� �̻� ��ġ�Ǵ� ���, ���� ���� ť�� ����
                if(vtDup.x > 2 || vtDup.y > 2 || vtDup.z > 2)
                {
                    unUsedBlocks.Enqueue(blockInfo);
                    useQueue = isListComplete || !useQueue;

                    continue;
                }

                // 6. ��ġ�� �� �ִ� ���, �ش� ��ġ�� �� �̵�
                block.xDuplicate = vtDup.x;
                block.yDuplicate = vtDup.y;
                block.zDuplicate = vtDup.z;
                if(block.blockObj != null)
                {
                    block.Move(row, col);
                }

                // 7. ã�� �� ����
                return block;
            }
        }

        private BlockVector NextBlock(bool useQueue)
        {
            if (useQueue && unUsedBlocks.Count > 0)
                return unUsedBlocks.Dequeue();

            if (!isListComplete && it.MoveNext())
                return it.Current.Value;

            return new BlockVector(null, Vector2Int.zero);
        }

        private Vector3Int CalcDuplications(int row, int col, Block block)
        {
            int xDup = 1, yDup = 1, zDup = 1;

            // ��
            if (col > 0 && map.blocks[row, col - 1].IsSafeEqual(block))
                yDup += map.blocks[row, col - 1].yDuplicate;

            // ��
            if (col < map.height-1 && map.blocks[row, col + 1].IsSafeEqual(block))
            {
                Block upBlock = map.blocks[row, col + 1];
                yDup += upBlock.yDuplicate;

                if (upBlock.yDuplicate == 1)
                    upBlock.yDuplicate = 2;
            }

            if (row % 2 == 1)
            {
                // LeftSide
                if(row > 0)
                {
                    if(col < map.height -1)
                    {
                        // �� 
                        Block leftTop = map.blocks[row + HexDirection.oddDirectionOffset[2].x, col + HexDirection.oddDirectionOffset[2].y];
                        if (row > 0 && leftTop.IsSafeEqual(block))
                            xDup += leftTop.xDuplicate;
                    }

                    // ��
                    Block leftBottom = map.blocks[row + HexDirection.oddDirectionOffset[1].x, col + HexDirection.oddDirectionOffset[1].y];
                    if (row > 0 && leftBottom.IsSafeEqual(block))
                        xDup += leftBottom.xDuplicate;
                }
                if(row < map.width - 1)
                {
                    if( col < map.height -1)
                    {
                        // �� 
                        Block rightTop = map.blocks[row + HexDirection.oddDirectionOffset[4].x, col + HexDirection.oddDirectionOffset[4].y];
                        if (row < map.width - 1 && rightTop.IsSafeEqual(block))
                        {
                            xDup += rightTop.xDuplicate;

                            if (rightTop.xDuplicate == 1)
                                rightTop.xDuplicate = 2;
                        }
                    }

                    // ��
                    Block rightBottom = map.blocks[row + HexDirection.oddDirectionOffset[5].x, col + HexDirection.oddDirectionOffset[5].y];
                    if (row < map.width - 1 && rightBottom.IsSafeEqual(block))
                    {
                        xDup += rightBottom.xDuplicate;

                        if (rightBottom.xDuplicate == 1)
                            rightBottom.xDuplicate = 2;
                    }
                }
            }
            else if (row % 2 == 0)
            {
                if(row > 0)
                {
                    // �� 
                    Block leftTop = map.blocks[row + HexDirection.evenDirectionOffset[2].x, col + HexDirection.evenDirectionOffset[2].y];
                    if (row > 0 && leftTop.IsSafeEqual(block))
                        zDup += leftTop.zDuplicate;

                    // ��
                    Block leftBottom = map.blocks[row + HexDirection.evenDirectionOffset[1].x, col + HexDirection.evenDirectionOffset[1].y];
                    if (row > 0 && leftBottom.IsSafeEqual(block))
                        zDup += leftBottom.zDuplicate;
                }
                
                if(row < map.width - 1)
                {
                    // �� 
                    Block rightTop = map.blocks[row + HexDirection.evenDirectionOffset[4].x, col + HexDirection.evenDirectionOffset[4].y];
                    if (row < map.width - 1 && rightTop.IsSafeEqual(block))
                        zDup += rightTop.zDuplicate;

                    // ��
                    Block rightBottom = map.blocks[row + HexDirection.evenDirectionOffset[5].x, col + HexDirection.evenDirectionOffset[5].y];
                    if (row < map.width - 1 && rightBottom.IsSafeEqual(block))
                        zDup += rightBottom.zDuplicate;
                }
            }
            return new Vector3Int(xDup, yDup, zDup);
        }
    }
}


