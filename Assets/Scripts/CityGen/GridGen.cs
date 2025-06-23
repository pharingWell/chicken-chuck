using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

namespace CityGen
{
    struct SplineObject
    {
        public LoftRoadBehaviour LoftScript;
        public SplineContainer Container;
        public GameObject ObjectRef;

        SplineObject(LoftRoadBehaviour loftScript, SplineContainer splineContainer, GameObject gameObject)
        {
            LoftScript = loftScript;
            Container = splineContainer;
            ObjectRef = gameObject;
        }


        public static SplineObject MakeNewSplineObject(int id, Mesh mesh, Material material, int segmentsPerMeter,
            int textureScale)
        {
            GameObject splineObject = new GameObject("Spline Container (" + id + ")");
            SplineContainer container = splineObject.AddComponent<SplineContainer>();
            splineObject.AddComponent<MeshRenderer>();
            splineObject.AddComponent<MeshFilter>();
            LoftRoadBehaviour loftScript = splineObject.AddComponent<LoftRoadBehaviour>();
            loftScript.Init(mesh, material, segmentsPerMeter, textureScale);
            return new SplineObject(loftScript, container, splineObject);
        }
    }

    public class GridGen : MonoBehaviour
    {
        [SerializeField] private int gridColumns = 40;

        [SerializeField] private int gridRows = 40;

        [SerializeField] private int tileSize;

        [SerializeField] float deadEndPercent = 10f;

        [SerializeField] private Vector3 position = Vector3.zero;

        [Header("Road Setup")] [SerializeField]
        private Mesh roadMesh;

        [SerializeField] private Material roadMaterial;
        [SerializeField] private int roadSegmentsPerMeter = 30;
        [SerializeField] private int roadTextureScale = 60;

        private SplineContainer _verticalSplinesContainer;
        private SplineContainer _horizontalSplinesContainer;
        private SplineContainer _diagonalSplinesContainer;

        private JobHandle _gridGenJobHandle;
        private NativeArray<Vector3> _positionsNativeArray;
        private NativeArray<bool> _splitsNativeArray;
        private int _jobStage = 0;

        private void Start()
        {
            _jobStage = 0;
            // length: vertical 
            _positionsNativeArray = new NativeArray<Vector3>(gridColumns * gridRows * 2, Allocator.Persistent);
            _splitsNativeArray = new NativeArray<bool>(_positionsNativeArray.Length, Allocator.Persistent);
            GridSplines splineJob = new GridSplines()
            {
                GridColumns = gridColumns,
                GridRows = gridRows,
                TileSize = tileSize,
                DeadEndPercent = deadEndPercent,
                Position = position,
                KnotPositions = _positionsNativeArray,
                Enabled = _splitsNativeArray,
                RandomSeed = (uint)DateTime.Now.Ticks
            };
            
            _gridGenJobHandle = splineJob.ScheduleByRef(_positionsNativeArray.Length,
                1);
            _jobStage = 1;
            
        }



        private void Update()
        {
            if (_jobStage == 1)
            {
                _gridGenJobHandle.Complete();
                _jobStage = 2;
            }
            else if (_gridGenJobHandle.IsCompleted && _jobStage == 2) {
                int index = 0;
                SplineObject objH = MakeNewSplineObject(0);
                int increment = gridColumns;
                while (index < gridColumns * gridRows) //horizontal
                {
                    IReadOnlyList<int> splits =
                        _splitsNativeArray.Skip(index).Take(gridColumns)
                            .Select((val, ind) => new { val, ind })
                            .Where(x => x.val)
                            .Select(x => x.ind)
                            .ToList();
                    IReadOnlyList<BezierKnot> knots = _positionsNativeArray.Skip(index).Take(gridColumns)
                        .Select(v3 => new BezierKnot(v3, Vector3.right, Vector3.right))
                        .ToList();
                    objH.Container.AddSpline(new Spline(new NativeSpline(knots, splits, false, float4x4.identity)));
                    index += gridColumns;
                }
                objH.LoftScript.LoftAllRoads();
                
                //vertical
                SplineObject objV = MakeNewSplineObject(1);
                while (index < gridColumns * gridRows * 2) //horizontal
                {
                    IReadOnlyList<int> splits =
                        _splitsNativeArray.Skip(index).Take(gridRows)
                            .Select((val, ind) => new { val, ind })
                            .Where(x => x.val)
                            .Select(x => x.ind)
                            .ToList();
                    IReadOnlyList<BezierKnot> knots = _positionsNativeArray.Skip(index).Take(gridRows)
                        .Select(v3 => new BezierKnot(v3, Vector3.right, Vector3.right))
                        .ToList();
                    objV.Container.AddSpline(new Spline(new NativeSpline(knots, splits, false, float4x4.identity)));
                    index += gridRows;
                }
                objV.LoftScript.LoftAllRoads();
                _jobStage = 3;
                _positionsNativeArray.Dispose();
                _splitsNativeArray.Dispose();
            }
        }

        SplineObject MakeNewSplineObject(int id)
        {
            return SplineObject.MakeNewSplineObject(id, roadMesh, roadMaterial, roadSegmentsPerMeter, roadTextureScale);
        }
    }
}