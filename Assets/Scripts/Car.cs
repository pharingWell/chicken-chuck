using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class Car : MonoBehaviour
    {
        private float speed = 10f;
        private int turn = -2;
        private readonly int blockTiles = 5;
        private readonly float tileScale = 10f;
        private float damping = 2.5f;
        private Quaternion _rotation;

        private void Start()
        {
        }

        private void Update()
        {
            gameObject.transform.position += gameObject.transform.forward * (Time.deltaTime * speed);
            Vector3 local = new Vector3(gameObject.transform.position.x % (tileScale * blockTiles), 0,
                gameObject.transform.position.z % (tileScale * blockTiles));
                          ;
            if ((blockTiles * tileScale / 2f * new Vector3(1f, 0f, 1f) - local).magnitude <
                tileScale / 2f && turn == -2)
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

                turn = -2;
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, _rotation, Time.deltaTime * damping);
        }
    }
}