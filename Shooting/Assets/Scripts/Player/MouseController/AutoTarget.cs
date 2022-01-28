using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTarget : MonoBehaviour
{
    GameObject player;
    PlayerController p;

    public GameObject targetMark;

    Vector3 oriPos;

    void Start()
    {
        //TargetIndicator의 target정보를 playerController로 전달하기위해
        player = GameObject.FindGameObjectWithTag("Player");
        p = player.GetComponent<PlayerController>();

        //게임시작시 targetMark의 첫위치를 기억
        oriPos = targetMark.transform.position;
    }

    //타겟인디케이터의 타겟마크가 화면 중앙의 원안에 머무를경우
    //자동으로 해당 타겟으로 타게팅되며 Targetindicator의 Target정보를 playerContoller에 전달
    //playerContoller는 laserBlast스크립트에 target을 전달
    //laserBlast스크립트는 전달받은 target을 향해 공격
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Target"))
        {
            //targetindicator에섯 target을 가져오기 위해
            TargetIndicator ti = collision.GetComponent<TargetIndicator>();

            //targetMark 위치를 타겟인디케이터마크의 위치로 lerp로 이동시킴
            targetMark.transform.position = Vector3.Lerp(targetMark.transform.position, collision.transform.position, 3.0f * Time.deltaTime);

            p.target = ti.target;
        }
    }
    //화면 중앙의 원에서 타겟마크가 벗어날경우
    //타겟을 null로 변경시키고 타겟마크를 중앙으로 이동
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Target"))
        {
            targetMark.transform.position = oriPos;

            p.target = null;
        }
    }
}
