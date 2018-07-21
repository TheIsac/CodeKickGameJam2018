using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    public void SetToSmallArena()
    {
        transform.position = new Vector3(0, 15, 0);
    }

    public void SetToLargeArena()
    {
        transform.position = new Vector3(0, 20, 0);
    }

    public void SetToGiganticArena()
    {
        transform.position = new Vector3(0, 40, 0);
    }
}