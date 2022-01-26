using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluePrintRotation : MonoBehaviour
{
    public GameObject Player;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rot = new Vector3(0, 0, Player.transform.eulerAngles.z);

        transform.localRotation = Quaternion.Euler(rot);   
    }
}
