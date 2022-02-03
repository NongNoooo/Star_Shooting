using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCamMove : MonoBehaviour
{
    public GameObject playerMiniMapSphere;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(playerMiniMapSphere.transform.position.x, playerMiniMapSphere.transform.position.y + 6.5f, playerMiniMapSphere.transform.position.z);
    }
}
