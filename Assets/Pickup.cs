using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Pickup : MonoBehaviour
{
    public bool isPickup;

    [SerializeField] private Vector3 mapCenter;

    private readonly int toEdge = 4;

    private readonly int tileSize = 50;

    private Vector3 mapBL;
    // Start is called before the first frame update
    void Start()
    {
        mapBL = mapCenter - new Vector3(tileSize / 2f, 0, tileSize / 2f) - toEdge * new Vector3(tileSize, 0, tileSize);
        gameObject.transform.position = mapCenter - new Vector3(25f, 0, 25f);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPickup)
        {
            foreach (Transform child in gameObject.transform)
            {
                if(child.gameObject.name != "Start")
                    child.gameObject.SetActive(false);
                else
                    child.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (Transform child in gameObject.transform)
            {
                if(child.gameObject.name != "End")
                    child.gameObject.SetActive(false);
                else
                    child.gameObject.SetActive(true);
            }
        }
    }

    public void toRandomLoc()
    {
        gameObject.transform.position = new Vector3(tileSize * Random.Range(0, toEdge * 2), 0,
            tileSize * Random.Range(0, toEdge * 2));
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //other.gameObject.TryGetComponent();
            isPickup = !isPickup;
            toRandomLoc();
        }
    }
}
