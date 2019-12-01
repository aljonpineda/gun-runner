using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drill : MonoBehaviour
{
    public Animator Drillanimator;
    public ParticleSystem DrillDirt;
    private bool DrillStarted;
    public bool IsDrillStarted { get { return DrillStarted; } }

    private void FixedUpdate()
    {
        if (DrillStarted)
        {
            transform.position += Vector3.right * Time.deltaTime;
            transform.position += Vector3.down * Time.deltaTime;

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "s")
        {
            CameraManager.Instance.ZoomIntoPlayer();
            StartExit();
        }
    }

    void StartExit()
    {
        Drillanimator.SetBool("ExitStarted", true);
        DrillStarted = true;
        CreateDrillDirt();
    }

    void CreateDrillDirt()
    {
        DrillDirt.Play();
    }
}
