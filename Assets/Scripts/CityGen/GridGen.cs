
using System;
using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
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
        
        
        
        public static SplineObject MakeNewSplineObject(int id, Mesh mesh, Material material, int segmentsPerMeter, int textureScale)
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
        private NativeArray<NativeSpline> _resultNativeArray;

        private void Start()
        {
            _resultNativeArray = new NativeArray<NativeSpline>(gridColumns + gridRows, Allocator.Persistent);
            GridSplines splineJob = new GridSplines()
            {
                GridColumns = gridColumns,
                GridRows = gridRows,
                TileSize = tileSize,
                DeadEndPercent = deadEndPercent,
                Position = position,
                Result = _resultNativeArray
            };
            _gridGenJobHandle = splineJob.ScheduleByRef(_resultNativeArray.Length,
                16);
        }

        private void Update()
        {
            if (_gridGenJobHandle.IsCompleted)
            {
                int index = 0;
                SplineObject objV = MakeNewSplineObject(0);
                for (; index < gridColumns; index++)
                {
                    objV.Container.AddSpline(new Spline(_resultNativeArray[index]));
                }

                objV.LoftScript.LoftAllRoads();
                SplineObject objH = MakeNewSplineObject(1);
                for (; index < gridRows; index++)
                {
                    objH.Container.AddSpline(new Spline(_resultNativeArray[index]));
                }

                objH.LoftScript.LoftAllRoads();
                // _resultNativeArray.Dispose();
            }
        }

        SplineObject MakeNewSplineObject(int id)
        {
            return SplineObject.MakeNewSplineObject(id, roadMesh, roadMaterial, roadSegmentsPerMeter, roadTextureScale);
        }

    }
}