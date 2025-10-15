using UnityEngine;

public class UIFollowObj : MonoBehaviour
{
    public Transform obj;
    void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(obj.position);
    }
}
