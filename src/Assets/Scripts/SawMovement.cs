using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawMovement : MonoBehaviour
{
    public Rigidbody2D m_rigidbody;
    public float travelLength;
    public bool VerticalSaw;
    private float distanceTravelled = 0;
    Vector3 lastPosition;
    public float speed=-1;

    void Start()
    {
        lastPosition = transform.position;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (VerticalSaw==false)
        {
            if (travelLength >= distanceTravelled)
            {
                m_rigidbody.velocity = new Vector3(speed, 0, 0);
                distanceTravelled += Vector3.Distance(transform.position, lastPosition);
                lastPosition = transform.position;
            }
            else
            {
                speed *= -1;
                distanceTravelled = 0;
            }

        }
        else
        {
            if (travelLength >= distanceTravelled)
            {
                m_rigidbody.velocity = new Vector3(0, speed, 0);
                distanceTravelled += Vector3.Distance(transform.position, lastPosition);
                lastPosition = transform.position;
            }
            else
            {
                speed *= -1;
                distanceTravelled = 0;
            }


        }


        

    }

    }
