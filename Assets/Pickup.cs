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
        mapBL = mapCenter - 3.5f * new Vector3(tileSize, 0, tileSize);
        gameObject.transform.position = mapCenter - new Vector3(25f, 0, 25f);
        isPickup = true;
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
        gameObject.transform.position = mapBL + new Vector3(tileSize * Random.Range(0, 8), 0,
            tileSize * Random.Range(0, toEdge * 2));
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.LogError("Entered");
            //other.gameObject.TryGetComponent();
            isPickup = !isPickup;
            toRandomLoc();
        }
    }
}
