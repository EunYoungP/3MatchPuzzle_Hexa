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
            // 각 블럭의 매칭 정보를 업데이트합니다.
            PrepareDuplicationDatas();

            // 셔플 대상 블럭을 리스트에 보관합니다.
            PrepareShuffleBlocks();

            // 준비한 데이터를 이용해 셔플을 수행합니다.
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
                    }   // 움직이지 않는 블럭 처리
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
                // 1. Queue에서 블럭을 하나 꺼냅니다. 첫번째 후보
                BlockVector blockInfo = NextBlock(useQueue);
                Block block = blockInfo.Key;

                // 2. 리스트에서 블럭을 모두 처리한 경우. : 전체 루프에서 1회 발생
                if(block == null)
                {
                    blockInfo = NextBlock(true);
                    block = blockInfo.Key;
                }

                if (prevVariety == BlockVariety.NULL)
                    prevVariety = block.blockVariety;

                // 3. 리스트를 모두 처리 한 경우
                if(isListComplete)
                {
                    if(firstBlock == null)
                    {
                        // 3-1. 리스트를 처리하고, 처음으로 큐에서 꺼낸 경우
                        firstBlock = block;
                    }
                    else if( ReferenceEquals(firstBlock, block))
                    {
                        // 3-2. 큐의 모든 블럭이 조건에 맞지 않아 첫번째 요소로 돌아온 경우
                        map.ChangeBlock(block, prevVariety);
                    }
                }

                // 4. 여섯 방향 인접 블럭과 연속되는 개수를 계산
                Vector3Int vtDup = CalcDuplications(row, col, block);

                // 5. 2개 이상 매치되는 경우, 현재 블럭을 큐에 보관
                if(vtDup.x > 2 || vtDup.y > 2 || vtDup.z > 2)
                {
                    unUsedBlocks.Enqueue(blockInfo);
                    useQueue = isListComplete || !useQueue;

                    continue;
                }

                // 6. 위치할 수 있는 경우, 해당 위치로 블럭 이동
                block.xDuplicate = vtDup.x;
                block.yDuplicate = vtDup.y;
                block.zDuplicate = vtDup.z;
                if(block.blockObj != null)
                {
                    block.Move(row, col);
                }

                // 7. 찾은 블럭 리턴
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

            // ↓
            if (col > 0 && map.blocks[row, col - 1].IsSafeEqual(block))
                yDup += map.blocks[row, col - 1].yDuplicate;

            // ↑
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
                        // ↖ 
                        Block leftTop = map.blocks[row + HexDirection.oddDirectionOffset[2].x, col + HexDirection.oddDirectionOffset[2].y];
                        if (row > 0 && leftTop.IsSafeEqual(block))
                            xDup += leftTop.xDuplicate;
                    }

                    // ↙
                    Block leftBottom = map.blocks[row + HexDirection.oddDirectionOffset[1].x, col + HexDirection.oddDirectionOffset[1].y];
                    if (row > 0 && leftBottom.IsSafeEqual(block))
                        xDup += leftBottom.xDuplicate;
                }
                if(row < map.width - 1)
                {
                    if( col < map.height -1)
                    {
                        // ↗ 
                        Block rightTop = map.blocks[row + HexDirection.oddDirectionOffset[4].x, col + HexDirection.oddDirectionOffset[4].y];
                        if (row < map.width - 1 && rightTop.IsSafeEqual(block))
                        {
                            xDup += rightTop.xDuplicate;

                            if (rightTop.xDuplicate == 1)
                                rightTop.xDuplicate = 2;
                        }
                    }

                    // ↘
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
                    // ↖ 
                    Block leftTop = map.blocks[row + HexDirection.evenDirectionOffset[2].x, col + HexDirection.evenDirectionOffset[2].y];
                    if (row > 0 && leftTop.IsSafeEqual(block))
                        zDup += leftTop.zDuplicate;

                    // ↙
                    Block leftBottom = map.blocks[row + HexDirection.evenDirectionOffset[1].x, col + HexDirection.evenDirectionOffset[1].y];
                    if (row > 0 && leftBottom.IsSafeEqual(block))
                        zDup += leftBottom.zDuplicate;
                }
                
                if(row < map.width - 1)
                {
                    // ↗ 
                    Block rightTop = map.blocks[row + HexDirection.evenDirectionOffset[4].x, col + HexDirection.evenDirectionOffset[4].y];
                    if (row < map.width - 1 && rightTop.IsSafeEqual(block))
                        zDup += rightTop.zDuplicate;

                    // ↘
                    Block rightBottom = map.blocks[row + HexDirection.evenDirectionOffset[5].x, col + HexDirection.evenDirectionOffset[5].y];
                    if (row < map.width - 1 && rightBottom.IsSafeEqual(block))
                        zDup += rightBottom.zDuplicate;
                }
            }
            return new Vector3Int(xDup, yDup, zDup);
        }
    }
}


