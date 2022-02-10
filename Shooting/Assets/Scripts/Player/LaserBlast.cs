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
            //Ÿ���� NULL�϶� ������������ ����(laserBlaster������Ʈ�� ����)�� vector3 forword �������� ������ �߻�
            transform.Translate(Vector3.forward * 4000 * Time.deltaTime);
        }
        else if(target != null)
        {
            //Ÿ���� null�� �ƴҶ�(autoTarget��ũ��Ʈ���� targetindicator�� Target�� �� ��ũ��Ʈ�� ����)

            //Ÿ���� �������� �������� �߻�
            Vector3 dir = target.transform.position - transform.position;

            transform.position += dir.normalized * 4000 * Time.deltaTime;
            transform.LookAt(target.transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("�� ����");
            Destroy(this.gameObject);
        }
    }
}
