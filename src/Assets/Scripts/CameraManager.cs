using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [SerializeField] private CinemachineVirtualCamera initialVcam;
    [SerializeField] private CinemachineVirtualCamera finalVcam;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        initialVcam.enabled = true;
        finalVcam.enabled = false;
    }

    public void ZoomIntoPlayer()
    {
        finalVcam.enabled = true;
    }
}
