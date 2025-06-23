using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class Pickup : MonoBehaviour
{
    public bool isPickup;

    [SerializeField] private Vector3 mapCenter;

    private readonly int toEdge = 4;

    private readonly int tileSize = 50;

    private Vector3 mapBL;

    public Text fuckingChickenText;
    public Image arrow;
    public GameObject cargo;

    // Start is called before the first frame update
    void Start()
    {
        fuckingChickenText.enabled = false;
        cargo.SetActive(false);
        mapBL = mapCenter - 3.5f * new Vector3(tileSize, 0, tileSize);
        gameObject.transform.position = mapCenter - new Vector3(25f, 0, 25f);
        isPickup = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (fuckingChickenText.enabled) 
        {
            if ((Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2)))
            {
                fuckingChickenText.enabled = false;
                arrow.enabled = true;
                toRandomLoc();
            }
        }
        else if (isPickup)
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
        Vector3 pos = gameObject.transform.position;
        while ((gameObject.transform.position - pos).magnitude < 50)
        {
            pos = mapBL + new Vector3(tileSize * Random.Range(0, 8), 0,
                tileSize * Random.Range(0, toEdge * 2));
        }

        gameObject.transform.position = pos;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //other.gameObject.TryGetComponent();

            if (!isPickup)
            {
                fuckingChickenText.enabled = true;
                arrow.enabled = false;
                cargo.SetActive(false);
                foreach (Transform child in gameObject.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
            else
            {
                toRandomLoc();
                cargo.SetActive(true);
            }
            isPickup = !isPickup;
        }
    }
}
