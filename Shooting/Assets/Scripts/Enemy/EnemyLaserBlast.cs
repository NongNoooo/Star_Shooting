using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaserBlast : MonoBehaviour
{
    public GameObject target;

    public float laserDamage;

    public Vector3 dir;

    void Start()
    {
        if(target == null)
        {
            return;
        }

        transform.LookAt(target.transform.position);
        dir = (target.transform.position - transform.position).normalized;
    }

    void Update()
    {
        transform.position += dir * 4000 * Time.deltaTime;

        if(target == null)
        {
            transform.position += transform.forward * 4000 * Time.deltaTime;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("피격");

            PlayerController pc = other.GetComponent<PlayerController>();

            pc.Damaged(laserDamage);
        }

        if (other.CompareTag("Environment"))
        {
            Destroy(this.gameObject);
        }
    }
}
