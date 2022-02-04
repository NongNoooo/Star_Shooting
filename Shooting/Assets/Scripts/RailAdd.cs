using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailAdd : MonoBehaviour
{
    public GameObject[] railChild;


    void Awake()
    {
        railChild = new GameObject[transform.childCount];

        Debug.Log(transform.childCount);
        for(int i = 0; i < transform.childCount; i++)
        {
            railChild[i] = transform.GetChild(i).gameObject;
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
