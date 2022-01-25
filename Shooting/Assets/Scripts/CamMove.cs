using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMove : MonoBehaviour
{
    public GameObject CamPos;

    void Awake()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        CamPos = p.transform.GetChild(1).gameObject;
    }

    void Update()
    {
        CMOve();
        CRotation();
    }

    void CMOve()
    {
        transform.position = Vector3.Slerp(transform.position, CamPos.transform.position, 1.0f * Time.deltaTime);
    }
    
    void CRotation()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, CamPos.transform.rotation, 1.0f * Time.deltaTime);
    }
}
