using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class Car : MonoBehaviour
    {
        private float speed = 10f;
        private int turn = -3;
        private readonly int blockTiles = 5;
        private readonly float tileScale = 10f;
        private float damping = 2.5f;
        private Quaternion _rotation;

        private void Start()
        {
            turn = -3;
        }

        private void FixedUpdate()
        {
            gameObject.transform.position += gameObject.transform.forward * (Time.deltaTime * speed);
            Vector3 local = new Vector3(gameObject.transform.position.x % 50f, 0,
                gameObject.transform.position.z % 50f);
            if (local.magnitude is < 5f or > 45f && turn == -2)
            {
                turn = Random.Range(-1, 2);
                _rotation = gameObject.transform.rotation;
                _rotation *= Quaternion.Euler(0, 90 * turn, 0); // this adds a 90 degrees Y rotation
                if (turn == -1)
                {
                    damping = 2.1f;
                } else if (turn == 1)
                {
                    damping = 4.5f;
                } 
            }
            else if (local.magnitude is > 20f and < 30f)    
            {                                               
                turn = -2;                                  
            }                                               
            transform.rotation = Quaternion.Slerp(transform.rotation, _rotation, Time.deltaTime * damping);
        }
    }
}