using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 10.0f;
    public float turnSpeed = 0.2f;
    [Range(0,20.0f)]
    public float laserDamage = 10.0f;
    [Range(0,20.0f)]
    public float shield = 10.0f;


    public enum DogFightState
    {
        Chasing,
        Chased,
        FaceToFace,
        DoNothing,
    }

    public enum EnemySightCondition
    {
        LookAtPlayer,
        NotLookAtPlayer,
    }

    public enum PlayerSightCondition
    {
        InPlayerSight,
        NotInPlayerSight,
    }

    public enum PlayerAimCondition
    {
        PlayerAimedMe,
        PlayerNotAimedMe,
    }

    public DogFightState dogFightState;
    public EnemySightCondition enemySightCondition;
    public PlayerSightCondition playerSightCondition;
    public PlayerAimCondition playerAimCondition;

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

    float dis;


    void Update()
    {
        //플레이어로의 방향 찾음
        dir = (player.transform.position - transform.position).normalized;
        //자신이 플레이어의 카메라 범위안에 있는지 체크
        //참고* playerDir의 x,y,z가 0보다 크면 적이 카메라 범위 안에 있는것
        playerDir = Camera.main.WorldToViewportPoint(transform.position);
        //자신과 플레이어간의 거리를 구함

        dis = Vector3.Distance(transform.position, player.transform.position);

        //항상 앞으로 이동
        //아래의 스위치문들의 상태에 따라
        //속도를 줄이고 방향을 지정해서 움직임을 달리하게 만듬
        transform.position += transform.forward * moveSpeed * 100.0f * Time.deltaTime;


        //Debug.Log(dir.z);
        //RotationAngle();
        PlayerSightStateChanger();
        EnemySightStateChanger();
        DogFightStateChanger();

        //플레이어의 시야내에 자신(적)이 있는지 판단
        PlayerSightState();
        //자신(적)의 시야내에 플레이어가 있는지 판단
        EnemySightState();
        //플레이어가 자신(적)을 조준하고 있는지 판단
        PlayerAimState();

        //위 3개의 스위치 문의 상태에 따라 자신의 움직임을 결정하는 스위치문
        DogFight();

    }

    //시야 범위
    float sightAngle = 50f;
    float playerSightAngle = 70.0f;
    //자신(적)의 시야 범위 내에 플레이어가 있는지 없는지 판단
    void EnemySightStateChanger()
    {
        //자신의 forword방향과 플레이어를 바라볼때의 각도
        float dot = Vector3.Dot(transform.forward, dir);

        //위에서 계산한 dot을
        //내적을 이용한 각 계산하기
        //thetha = cos^-1( a dot b / |a||b|)
        float theta = Mathf.Acos(dot) * Mathf.Rad2Deg;

        if (theta <= sightAngle)
        {
            //Debug.Log("플레이어 바라보는중");
            enemySightCondition = EnemySightCondition.LookAtPlayer;
        }
        else
        {
            enemySightCondition = EnemySightCondition.NotLookAtPlayer;
        }
    }
    //플레이어의 시야범위 내에 자신(적)이 있는지 없는지 판단
    void PlayerSightStateChanger()
    {
        float dot = Vector3.Dot(player.transform.forward, (transform.position - player.transform.position).normalized);

        //내적을 이용한 각 계산하기
        // thetha = cos^-1( a dot b / |a||b|)
        float theta = Mathf.Acos(dot) * Mathf.Rad2Deg;

        //Debug.Log("타겟과 AI의 각도 : " + theta);
        if (theta <= playerSightAngle)
        {
            //Debug.Log("플레이어 적 바라보는중");
            playerSightCondition = PlayerSightCondition.InPlayerSight;
        }
        else
        {
            playerSightCondition = PlayerSightCondition.NotInPlayerSight;
        }
    }
    //
    void DogFightStateChanger()
    {
        //dir.z가 0보다 큼
        //플레이어가 자신의 앞에 있는것
        if (dir.z > 0)
        {
            //플레이어가 자신을 바라보고있지 않을때 
            if (playerSightCondition == PlayerSightCondition.NotInPlayerSight)
            {
                //Debug.Log("뒤쫒는중");
                dogFightState = DogFightState.Chasing;
            }
            else if(playerSightCondition == PlayerSightCondition.InPlayerSight)
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

    void EnemySightState()
    {
        switch (enemySightCondition)
        {
            case EnemySightCondition.LookAtPlayer:
                break;
            case EnemySightCondition.NotLookAtPlayer:
                //플레이어가 시야범위 밖에 있음
                SpeedDown();
                break;
            
        }
    }
    
    void PlayerSightState()
    {
        switch (playerSightCondition)
        {
            case PlayerSightCondition.InPlayerSight:
                break;
            case PlayerSightCondition.NotInPlayerSight:
                break;
        }
    }

    void PlayerAimState()
    {
        switch (playerAimCondition)
        {
            case PlayerAimCondition.PlayerAimedMe:
                break;
            case PlayerAimCondition.PlayerNotAimedMe:
                break;
        }
    }

    void Chase()
    {
        Debug.Log("Chase");
        //플레이어와의 거리가 1000보다 크면 플레이어 방향으로 이동
        if (dis > 500.0f)
        {
            //기체를 플레이어의 방향으로 서서히 돌림
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);

        }
    }


    float curTime = 0;
    void Escape()
    {
        Debug.Log("Escape");
        curTime += Time.deltaTime;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);

    }

    void PassBy()
    {

        if(dis < 300.0f)
        {
            TurnRightUp();
        }
    }

    float zRot;
    float xRot;

    void RotIncrease()
    {
        //Debug.Log(zRot);

        if (zRot > 45 && xRot > 25)
        {
            return;
        }

        zRot = 4500.0f * Time.deltaTime;
        xRot = 2500.0f * Time.deltaTime;
    }

    void TurnRightUp()
    {
        Debug.Log(dir.z);

        float yRot = transform.rotation.y + 2.0f * Time.deltaTime;

        //transform.rotation = Quaternion.Euler(xRot, yRot,transform.rotation.z);

        RotIncrease();

        //transform.Rotate(transform.rotation.x, transform.rotation.y, transform.rotation.z - zRot);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.x - xRot, transform.rotation.y, transform.rotation.z - zRot), Time.deltaTime);

        transform.position += transform.up * 30 * Time.deltaTime;
    }
    

    void SpeedDown()
    {
        moveSpeed -= 1.0f * Time.deltaTime;
        turnSpeed += 0.01f * Time.deltaTime;

        if(playerSightCondition == PlayerSightCondition.InPlayerSight)
        {
            shield += 1.0f * Time.deltaTime;
        }
        else if(playerSightCondition == PlayerSightCondition.NotInPlayerSight)
        {
            laserDamage += 1.0f * Time.deltaTime;
        }
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
