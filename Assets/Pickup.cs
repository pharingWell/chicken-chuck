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

    private readonly float tileSize = 50f;

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
            pos = mapBL + new Vector3(tileSize * Random.Range(0, 8), 0f,
                tileSize * Random.Range(0, 8));
        }
        int side = Random.Range(0, 9);
        Quaternion rotation;
        var position = pos;
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
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
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
                gameObject.transform.position = mapBL * 2; //get it way out of the map
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
