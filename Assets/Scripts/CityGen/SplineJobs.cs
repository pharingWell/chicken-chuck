using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace CityGen
{
    [BurstCompile]
    public struct GridSplines : IJobParallelFor
    {
        // Input data for the job
        [Unity.Collections.ReadOnly] public int GridRows;
        [Unity.Collections.ReadOnly] public int GridColumns;
        [Unity.Collections.ReadOnly] public float DeadEndPercent;
        [Unity.Collections.ReadOnly] public Vector3 Position;
        [Unity.Collections.ReadOnly] public float TileSize;
        [Unity.Collections.ReadOnly] public uint RandomSeed;
        
        [WriteOnly]
        public NativeArray<Vector3> KnotPositions;
        public NativeArray<bool> Enabled;

        public void Execute(int index)
        {
            
            // vertical lines
            var rnd = new Unity.Mathematics.Random((uint)index + RandomSeed);
            if (index < GridColumns * GridRows)
            {
                Horizontal(index, rnd);
            }
            else
            {
                Vertical(index, rnd);
            }
        }

        void Vertical(int index, Unity.Mathematics.Random rnd)
        {
            int y = index % GridRows;
            if (rnd.NextInt() % 100 >= DeadEndPercent)
            {
                KnotPositions[index] = Position + new Vector3( math.floor(index / (float)GridRows) - (GridRows + 2) / 2f, 0, y - (GridColumns + 2) / 2f) * TileSize;
                Enabled[index] = true;
            }
            else //dead end
            {
                KnotPositions[index] = Vector3.zero;
                Enabled[index] = false;
            }
        }
        
        void Horizontal(int index, Unity.Mathematics.Random rnd)
        {
            int x = index % GridColumns;
            if (rnd.NextInt() % 100 >= DeadEndPercent)
            {
                KnotPositions[index] = Position + new Vector3(x - (GridRows + 2) / 2f, 0, math.floor(index / (float)GridColumns) - (GridColumns + 2) / 2f) * TileSize;
                Enabled[index] = true;
            }
            else //dead end
            {
                KnotPositions[index] = Vector3.zero;
                Enabled[index] = false;
            }
        }
        
        // BezierKnot NewKnot(int x, int y, Vector3 direction)
        // {
        //     // 1 is added to the grid rows and columns on each side as the first roads are all dead ends and are not passable.
        //     return new BezierKnot(
        //     ) * TileSize, direction, direction);
        // }


    }
    
    


}