using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;



namespace CityGen
{
        //     [BurstCompile]: add burst to project
        
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
        [NativeDisableContainerSafetyRestriction]
        public NativeArray<NativeSpline> Result;
        
        // public GridSplines(int gridRows = 20, int gridColumns = 20, float tileSize = 40f, float deadEndPercent = 10, Vector3 position = default)
        // {
        //     GridRows = gridRows;
        //     GridColumns = gridColumns;
        //     DeadEndPercent = deadEndPercent;
        //     Position = position;
        //     Result = default;
        //     TileSize = tileSize;
        // }


        public void Execute(int index)
        {
            
            // vertical lines
            var positions = new List<Vector3>();
            var splits = new List<int>();
            var rnd = new Unity.Mathematics.Random((uint)index + RandomSeed);
            if (index < GridColumns)
            {
                Horizontal(index, rnd, out List<Vector3> knots, out List<int> splitsOut);
                positions.AddRange(knots);
                splits.AddRange(splitsOut);
            }
            else
            {
                Vertical(index, rnd, out List<Vector3> knots, out List<int> splitsOut);
                positions.AddRange(knots);
                splits.AddRange(splitsOut);
            }
           // Result[index] = //new NativeSpline(knots.AsReadOnly(), splits.AsReadOnly(), false, float4x4.identity);
           
          
        }

        void Vertical(int index, Unity.Mathematics.Random rnd, out List<Vector3> knotPositions, out List<int> splits)
        {
            splits = new List<int>();
            knotPositions = new List<Vector3>(GridRows);
            // TODO: To allow for only using one NativeArray, this needs to be broken down into a single piece.
            for (int y = 0; y < GridRows; y++)  //
            {
                if (rnd.NextInt() % 100 >= DeadEndPercent)
                {
                    knotPositions.Add(Position + new Vector3(index - (GridRows + 2) / 2f, 0, y - (GridColumns + 2) / 2f));
                }
                else //dead end
                {
                    knotPositions.Add(Vector3.zero);
                    splits.Add(y);
                }
            }
            splits.Add(-1); //end character
        }
        
        void Horizontal(int index, Unity.Mathematics.Random rnd, out List<Vector3> knotPositions, out List<int> splits)
        {
            splits = new List<int>();
            knotPositions = new List<Vector3>(GridColumns);
            for (int x = 0; x < GridColumns; x++)
            {
                if (rnd.NextInt() % 100 >= DeadEndPercent)
                {
                    knotPositions.Add(Position + new Vector3(x - (GridRows + 2) / 2f, 0, index - (GridColumns + 2) / 2f));
                }
                else //dead end
                {
                    knotPositions.Add(Vector3.zero);
                    splits.Add(x);
                }
            }
            splits.Add(-1); //end character
        }
        
        // BezierKnot NewKnot(int x, int y, Vector3 direction)
        // {
        //     // 1 is added to the grid rows and columns on each side as the first roads are all dead ends and are not passable.
        //     return new BezierKnot(
        //     ) * TileSize, direction, direction);
        // }


    }


}