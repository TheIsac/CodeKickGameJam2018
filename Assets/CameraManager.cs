using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private ArenaBoundariesController arenaBoundaries;
    private MainCameraController mainCamera;

    // Use this for initialization
    void Start()
    {
        arenaBoundaries = GetComponentInChildren<ArenaBoundariesController>();
        mainCamera = GetComponentInChildren<MainCameraController>();
    }

    public void SetToSmallArena()
    {
        arenaBoundaries.SetToSmallArena();
        mainCamera.SetToSmallArena();
    }

    public void SetToLargeArena()
    {
        arenaBoundaries.SetToLargeArena();
        mainCamera.SetToLargeArena();
    }
}