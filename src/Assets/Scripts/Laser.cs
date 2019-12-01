using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{

    private LineRenderer lineRenderer;
    public Transform LaserHit;
    public EdgeCollider2D edgeCollider;
    public GameObject CharacterController;
    public BoxCollider2D Coll; 
    private Vector3 Distance;
    public bool shootdown = false;
    



    private Vector3 startPos;    // Start position of line
    private Vector3 endPos;    // End position of line

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (shootdown==false)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, Mathf.Infinity, 1);
            LaserHit.position = hit.point;
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up*-1, Mathf.Infinity, 1);
            LaserHit.position = hit.point;
        }
       
        //Debug.DrawLine(transform.position, hit.point);
      
        
       
        
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, LaserHit.position);

        Distance = LaserHit.position - transform.position;
        transform.localScale = new Vector3(.2f,Distance.y*1.6f, 0);



    }


}
