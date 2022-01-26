using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMove : MonoBehaviour
{
    public GameObject CamPos;

    public Camera main;
    public Camera two;


    public GameObject player;

    void Awake()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
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
        transform.rotation = Quaternion.Slerp(transform.rotation, CamPos.transform.rotation, 2.0f * Time.deltaTime);
    }

    void CamTwoMove()
    {
        two.transform.position = player.transform.position;
    }
}
