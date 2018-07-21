using UnityEngine;

public class ArenaBoundariesController : MonoBehaviour
{
    public void SetToSmallArena()
    {
        transform.position = new Vector3(0, 0, 0);
        transform.localScale = new Vector3(.75f, 1, .75f);
    }

    public void SetToLargeArena()
    {
        transform.position = new Vector3(0, 0, 0);
        transform.localScale = new Vector3(1, 1, 1);
    }
}