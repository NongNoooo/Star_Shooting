using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBlast : MonoBehaviour
{
    public GameObject target;
    public float damage;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(target == null)
        {
            //??? null? ??? ??? ??
            //??? autotarget ????? targetindicator? ?? ???
            transform.Translate(Vector3.forward * 4000 * Time.deltaTime);
        }
        else if(target != null)
        {
            Vector3 dir = target.transform.position - transform.position;

            transform.position += dir.normalized * 4000 * Time.deltaTime;
            transform.LookAt(target.transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("? ??");

            EnemyController ec = other.GetComponent<EnemyController>();

            ec.Damaged(damage);

            Destroy(this.gameObject);
        }

        //���༺�� ȯ�湰ü�� �ε����� �������� ���������
        if (other.CompareTag("Environment"))
        {
            Destroy(this.gameObject);
        }
    }
}
