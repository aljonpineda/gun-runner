using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pivot : MonoBehaviour
{
    public CharacterController2D controller;
    public GameObject myPlayer;
    float test = 0;
    Vector3 FrozenDifference; 
    bool Frozen = false;
    // Update is called once per frame
    private void FixedUpdate()
    {

        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        // Find the Position of the Mouse

        //If Player Is Frozen ScreenShot Previous Mouse Position
        if (controller.Freezeplayer)
        {
            if (Frozen == false)
            {
                FrozenDifference = difference;
                difference.Normalize();
                Frozen = true;
            }
            
        }


            // Normalize the Value, Make beteen 0 and 1
            difference.Normalize();
        // Angle of our mouse so our arm points to that angle
        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        //If Player is frozen stop arm rotation
        if (controller.Freezeplayer)
        {
            rotationZ = Mathf.Atan2(FrozenDifference.y, FrozenDifference.x) * Mathf.Rad2Deg;
        }
            // Rotate the Arm to the mouse
            // If Facing Right
            if (controller.IsFacingRight())
        {
        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
        }
        // If Facing Left
        else
        {
        transform.rotation = Quaternion.Euler(0f, 180f, -rotationZ);
        }


        // Turn the arm rightside up even when the arm is on the other side of our body
        // If our arm is on the other side of the body
        if ((rotationZ < -90 || rotationZ > 90)) 
        {


            // If they are facing to the right
            if (controller.IsFacingRight() && !controller.TouchingWall)
            {

                //Flip the arm
                transform.localRotation = Quaternion.Euler(180, 0, -rotationZ);
                

            }
            // If facing Left
            else if (!controller.IsFacingRight() && !controller.TouchingWall)
            {

               // Flip on X and Mirror on Y
               transform.localRotation = Quaternion.Euler(180, 180, -rotationZ);
                

            }
            else if (controller.IsFacingRight() && controller.TouchingWall)
            {

                //Flip the arm
                transform.localRotation = Quaternion.Euler(180, 0, -rotationZ);


            }
            else if (!controller.IsFacingRight() && controller.TouchingWall)
            {

                // Flip on X and Mirror on Y
                //transform.localRotation = Quaternion.Euler(180, 180, -rotationZ);


            }


        }
      
        

    }
}
