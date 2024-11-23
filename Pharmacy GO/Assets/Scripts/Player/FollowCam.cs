using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public GameObject player;
    public Vector3 offset = new Vector3(0, 0, -10);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + offset;
    }
}
