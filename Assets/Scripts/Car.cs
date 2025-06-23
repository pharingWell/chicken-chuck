using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class Car : MonoBehaviour
    {
        private static int deadCars = 0;
        [SerializeField] private Vector3 mapCenter;
        [SerializeField] private Vector3 local;
        private float speed = 10f;
        private int turn;
        private bool canTurn = false;
        private readonly int blockTiles = 5;
        private readonly float tileScale = 10f;
        private float damping = 2.5f;
        private Quaternion _rotation;
        private MeshRenderer _renderer;
        [SerializeField] private GameObject Mesh;

        private void Start()
        {
            turn = 0;
            _renderer = Mesh.GetComponent<MeshRenderer>();
        }

        private bool outside = false;

        private void FixedUpdate()
        {
            gameObject.transform.position += gameObject.transform.forward * (Time.deltaTime * speed);
            transform.rotation = Quaternion.Slerp(transform.rotation, _rotation, Time.deltaTime * damping);

            Vector3 dirToCenter = (gameObject.transform.position - mapCenter).normalized;
            float angleRight = MathF.Max(Vector3.Dot(dirToCenter, gameObject.transform.right),
                Vector3.Dot(gameObject.transform.right, dirToCenter));
            float angleLeft = MathF.Max(Vector3.Dot(dirToCenter, -gameObject.transform.right),
                Vector3.Dot(-gameObject.transform.right, dirToCenter));
            // if (angleLeft < angleRight)
            // {
            //     _renderer.material.color = Color.red;
            // }
            // else
            // {
            //     _renderer.material.color = Color.green;
            // }
            local = new Vector3(gameObject.transform.position.x % 50f, 0, gameObject.transform.position.z % 50f);
            if(Math.Abs(gameObject.transform.position.x - mapCenter.x) < 3.45f * 50f &&
               Math.Abs(gameObject.transform.position.z - mapCenter.z) < 3.45f * 50)
            {
                outside = false; 
                _renderer.material.color = Color.white;
                _renderer.enabled = true;
            }
            else
            {
                outside = true;
                // _renderer.material.color = Color.blue;
                if (Math.Abs(gameObject.transform.position.x - mapCenter.x) > 3.85f * 50f ||
                    Math.Abs(gameObject.transform.position.z - mapCenter.z) > 3.85f * 50f)
                {
                    gameObject.transform.position += gameObject.transform.forward * (-7.5f * 50f);
                }
            }
            if (canTurn && Math.Abs(local.x) is < 5f or > 45f && Math.Abs(local.z) is < 5f or > 45f)
            {
                canTurn = false;
                if (outside)
                {
                    if (angleLeft < angleRight)
                    {
                        turn = -1;
                    }
                    else
                    {
                        turn = 1;
                    }
                }
                else
                {
                    turn = Random.Range(-1, 2);
                }
                if (turn != 0)
                {
                    _rotation = gameObject.transform.rotation;
                    _rotation *= Quaternion.Euler(0, 90f * turn, 0);
                    if (turn == -1)
                    {
                        damping = 2.1f;
                    }
                    else if (turn == 1)
                    {
                        damping = 4.5f;
                    }
                }
            }
            else if (Math.Abs(local.x) is > 15f and < 35f || Math.Abs(local.z) is > 15f and < 35f)
            {
                canTurn = true;
                // _renderer.material.color = Color.grey;
            }   else if (Math.Abs(25f - Math.Abs(local.x)) < 15f && Math.Abs(25f - Math.Abs(local.z)) < 15f)
            {
                
                if (speed > 0)
                {
                    Debug.LogWarning(++deadCars);
                }
                
                speed = 0f;
                
            }
            
        }
    }
}