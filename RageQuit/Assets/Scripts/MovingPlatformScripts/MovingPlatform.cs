using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform posA, posB;
    public float speed;
    Vector3 targetPos;

    PlayerMovement playerMovement;
    Rigidbody2D rb;
    Vector3 moveDirection;

    private void Start()
    {
        targetPos = posB.position;
        
    }

    private void Awake()
    {
       
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(Vector2.Distance(transform.position, posA.position) < 0.05f)
        {
            targetPos = posB.position;
           
        }

        if(Vector2.Distance(transform.position, posB.position) < 0.05f)
        {
            targetPos = posA.position;
            
        }
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }
}
