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
            //타겟이 NULL일땐 생성됬을때의 방향(laserBlaster오브젝트의 방향)의 vector3 forword 방향으로 레이저 발사
            transform.Translate(Vector3.forward * 4000 * Time.deltaTime);
        }
        else if(target != null)
        {
            //타겟이 null이 아닐때(autoTarget스크립트에서 targetindicator의 Target을 이 스크립트로 전달)

            //타겟의 방향으로 레이저를 발사
            Vector3 dir = target.transform.position - transform.position;

            transform.position += dir.normalized * 4000 * Time.deltaTime;
            transform.LookAt(target.transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("적 맞춤");
            Destroy(this.gameObject);
        }
    }
}
