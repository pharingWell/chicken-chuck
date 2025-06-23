using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] private Vector3 mapCenter;
    private Vector3 mapBL;
    public GameObject carPrefab;
    // Start is called before the first frame update
    void Start()
    {
        Random.InitState((int)DateTime.Now.Ticks);
        mapBL = mapCenter - 3.5f * new Vector3(50f, 0, 50f);
        for (int i = 0; i < 100; i++)
        {
            Vector3 intersection = mapBL + new Vector3(50f * Random.Range(0, 8), 0.82f,
                50f * Random.Range(0, 8));
            int side = Random.Range(0, 9);
            Quaternion rotation;
            var position = intersection;
            if (side < 4) //x axis
            {
                position.x += Random.Range(-30, 31);
                if (side % 2 == 0) //right side of the road
                {
                    position.z -= 1.5f;
                    rotation = Quaternion.LookRotation(Vector3.right);
                }
                else //left side of the road
                {
                    position.z += 1.5f;
                    rotation = Quaternion.LookRotation(Vector3.left);
                }
            }
            else //z axis
            {
                position.z += Random.Range(-30, 31);
                if (side % 2 == 0) //right side of the road
                {
                    position.x += 1.5f;
                    rotation = Quaternion.LookRotation(Vector3.forward);
                }
                else //left side of the road
                {
                    position.x -= 1.5f;
                    rotation = Quaternion.LookRotation(Vector3.back);
                }
            }
            Instantiate(carPrefab, position, rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
