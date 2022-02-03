using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum DogFightState
    {
        Chasing,
        Chased,
        FaceToFace,
        DoNothing,
    }

    public DogFightState dogFightState;

    GameObject player;

    public GameObject[] Rail;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        dogFightState = DogFightState.Chasing;

        Rail = GameObject.FindGameObjectsWithTag("Rail");
    }

    Vector3 dir;

    Vector3 playerDir;

    float mag;

    void Update()
    {
        //플레이어로의 방향 찾음
        dir = player.transform.position - transform.position;
        //자신이 플레이어의 카메라 범위안에 있는지 체크
        //참고* playerDir의 x,y,z가 0보다 크면 적이 카메라 범위 안에 있는것
        playerDir = Camera.main.WorldToViewportPoint(transform.position);
        //자신과 플레이어간의 거리를 구함
        mag = dir.magnitude;


        DogFightStateChanger();

        DogFight();

    }

    void DogFightStateChanger()
    {
        //dir.z가 0보다 큼
        //플레이어가 자신의 앞에 있는것
        if (dir.z > 0)
        {
            //플레이어가 자신을 바라보고있지 않을때 
            if (playerDir.z < 0)
            {
                //Debug.Log("뒤쫒는중");
                dogFightState = DogFightState.Chasing;
            }
            else
            {
                //Debug.Log("마주보는중");

                //플레이어를 바라보고있는 메인카메라의 범위안에 자신이 들어와있음
                // == 플레이어가 자신을 바라보고있음
                dogFightState = DogFightState.FaceToFace;
            }
        }

        //dir.z가 0보다 작기 때문에
        //플레이어가 자신의 뒤에있는것
        if (dir.z < 0)
        {
            //플레이어가 자신을 바라보고 있음
            if(playerDir.z > 0)
            {
                //Debug.Log("뒤쫒기는중");

                dogFightState = DogFightState.Chased;

            }
        }


        //Debug.Log(pos.normalized);
        //if(playerDir.x > 0 && playerDir.y > 0 && playerDir.z > 0)
        //{
        //    Debug.Log("카메라 안");
        //}
        //else if(playerDir.z < 0)
        //{
        //    Debug.Log("카메라 밖");
        //}
    }

    void DogFight()
    {
        switch (dogFightState)
        {
            case DogFightState.Chasing:
                Chase();
                break;
            case DogFightState.Chased:
                Escape();
                break;
            case DogFightState.FaceToFace:
                PassBy();
                break;
            case DogFightState.DoNothing:
                //RailRider();
                break;
        }
    }


    void Chase()
    {
        //플레이어와의 거리가 1000보다 크면 플레이어 방향으로 이동
        if(mag > 1000.0f)
        {
            //기체를 플레이어의 방향으로 서서히 돌림
            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 0.5f * Time.deltaTime);

            //플레이어의 방향으로 이동
            transform.position += dir.normalized * 100 * Time.deltaTime;
        }
        else
        {
            Debug.Log("붙기 직전");
            //플레이어와의 거리가 1000보다 작음으로 직선으로 이동하며 플레이어를 공격
            transform.position += transform.forward * 100 * Time.deltaTime;
        }
    }

    void Escape()
    {

    }

    void PassBy()
    {
        transform.position += transform.forward * 100 * Time.deltaTime;

        Debug.Log(mag);

        if(mag < 500.0f)
        {
            TurnRightUp();
        }
    }

    void TurnRightUp()
    {
        float xRot = transform.rotation.x - 2.0f * Time.deltaTime;
        float yRot = transform.rotation.y + 2.0f * Time.deltaTime;
        float zRot = transform.rotation.z - 3.0f * Time.deltaTime;


        //transform.rotation = Quaternion.Euler(xRot, yRot,transform.rotation.z);
        transform.Rotate(transform.rotation.x, transform.rotation.y, zRot);
        transform.position += transform.up * 50 * Time.deltaTime;
    }
    void TurnLeft()
    {

    }

    //public float[] dis;
    //public GameObject previousPos;

    //void FindRail()
    //{
    //    dis = new float[Rail.Length];
    //    for (int i = 0; i < Rail.Length; i++)
    //    {
    //        dis[i] = (Rail[i].transform.position - transform.position).magnitude;
    //    }

    //    float min = dis[0];
    //    previousPos = Rail[0];

    //    for (int j = 1; j < dis.Length; j++)
    //    {
    //        if (min > dis[j])
    //        {
    //            min = dis[j];
    //            previousPos = Rail[j];
    //        }
    //    }
    //}

    //void RailRider()
    //{
    //    FindRail();
    //
    //
    //}
}
