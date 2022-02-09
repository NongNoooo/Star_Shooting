using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaserBlast : MonoBehaviour
{
    public GameObject target;

    Vector3 dir;

    void Start()
    {
        dir = (target.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.Euler(dir);
        //transform.LookAt(target.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(dir * 4000 * Time.deltaTime);
    }

    /* private void OnCollisionEnter(Collision collision)
     {
         if (collision.gameObject.CompareTag("Player"))
         {
             Debug.Log("피격");
         }
     }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("피격");

            PlayerController pc = other.GetComponent<PlayerController>();
        }
    }
}
