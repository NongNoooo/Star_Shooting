using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Range(0,20.0f)]
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

    public enum RayHitCondition
    {
        SomethingHit,
        NothingHit,
    }

    public DogFightState dogFightState;
    public EnemySightCondition enemySightCondition;
    public PlayerSightCondition playerSightCondition;
    public PlayerAimCondition playerAimCondition;
    public RayHitCondition rayHitCondition;

    GameObject player;
    PlayerController pc;

    public float curTime = 0;
    public GameObject laserObj;
    public GameObject target;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        player = p.transform.Find("PlayerTargetPos").gameObject;
        pc = p.GetComponent<PlayerController>();

        dogFightState = DogFightState.Chasing;
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
        //transform.position += transform.forward * moveSpeed * 100.0f * Time.deltaTime;

        //공격 쿨타임
        LaserFireCount();
        LaserFire();

        //레이를 이용해 전방에 물체가 있는지 체크
        UseRayCheckDistanceChanger();
        RayDistanceState();

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
        //PlayerAimState상태 변경
        PlayerAimStateChanger();

        //위 3개의 스위치 문의 상태에 따라 자신의 움직임을 결정하는 스위치문
        DogFight();

    }


    //공격 쿨타임
    void LaserFireCount()
    {
        //curTime이 1보다 크면 리턴
        if(curTime >= 1.0f)
        {
            return;
        }
        curTime += Time.deltaTime;        
    }
    //공격
    void LaserFire()
    {
        if (target == null)
        {
            return;
        }

        if (curTime < 1.0f)
        {
            return;
        }

        GameObject l = Instantiate(laserObj);
        l.transform.position = transform.position;
        l.transform.forward = transform.forward;

        Debug.Log("적 공격");
        curTime = 0.0f;

        EnemyLaserBlast elb = l.GetComponent<EnemyLaserBlast>();

        elb.target = target;
    }

    //레이를 사용한 전방에 있는 물체 체크
    void UseRayCheckDistanceChanger()
    {
        RaycastHit hit;
        //레이 마스크 1~7번 레이어까지만 인식
        int mask = 1 << 8;

        //플레이어와 마주보고있는 상태일경우 
        if (dogFightState == DogFightState.FaceToFace)
        {
            
            Debug.DrawRay(transform.position, transform.forward * 500.0f, Color.red);

            if(Physics.Raycast(transform.position, transform.forward, out hit, 500.0f, mask))
            {
                Debug.Log("500 + " + hit.transform.name);
                rayHitCondition = RayHitCondition.SomethingHit;
            }
            else
            {
                rayHitCondition = RayHitCondition.NothingHit;
            }

            return;
        }


        //플레이어와 마주보고있는 상황이 아닐경우
        Debug.DrawRay(transform.position, transform.forward * 1000.0f, Color.red);

        if(Physics.Raycast(transform.position, transform.forward, out hit, 1000.0f, mask))
        {
            Debug.Log("1000 + " + hit.transform.name);
            rayHitCondition = RayHitCondition.SomethingHit;
        }
        else
        {
            rayHitCondition = RayHitCondition.NothingHit;
        }
    }

    void RayDistanceState()
    {
        switch (rayHitCondition)
        {
            case RayHitCondition.NothingHit:
                //RotReturn();
                break;
            case RayHitCondition.SomethingHit:
                Avoid();
                break;
        }
    }

    void Avoid()
    {
        float xRot = 2.0f * Time.deltaTime;
        float zRot = 2.0f * Time.deltaTime;

        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.x - xRot, 0, transform.rotation.z - zRot), 1.0f * Time.deltaTime);
        transform.rotation *= Quaternion.Euler(10 * Time.deltaTime, 0, 10 * Time.deltaTime);

        transform.position += transform.up * 30 * Time.deltaTime;
    }

    void RotReturn()
    {
        //if(transform.rotation.x)
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0, transform.rotation.y, 0)), Time.deltaTime);
    }

    //시야 범위
    float sightAngle = 20f;
    //자신(적)의 시야 범위 내에 플레이어가 있는지 없는지 판단
    float playerSightAngle = 45.0f;

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

            target = player;
        }
        else
        {
            enemySightCondition = EnemySightCondition.NotLookAtPlayer;

            target = null;
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
        //플레이어를 바라보고 있음
        if (enemySightCondition == EnemySightCondition.LookAtPlayer)
        {
            //플레이어가 자신을 바라보고있지 않을때 
            if (playerSightCondition == PlayerSightCondition.NotInPlayerSight)
            {
                //Debug.Log("뒤쫒는중");
                dogFightState = DogFightState.Chasing;
            }
        }

        //dir.z가 0보다 작기 때문에
        //플레이어가 자신의 뒤에있는것
        if (enemySightCondition == EnemySightCondition.NotLookAtPlayer)
        {
            //플레이어가 자신을 바라보고 있음
            if(playerSightCondition == PlayerSightCondition.InPlayerSight)
            {
                //Debug.Log("뒤쫒기는중");

                dogFightState = DogFightState.Chased;

            }
        }

        //자신(적)도 플레이어를 바라보고 있다면

        if (enemySightCondition == EnemySightCondition.LookAtPlayer)
        {
            //플레이어가 자신(적)을 조준하고 있을때
            if (playerAimCondition == PlayerAimCondition.PlayerAimedMe)
            {
                dogFightState = DogFightState.FaceToFace;
            }
            else if(playerAimCondition == PlayerAimCondition.PlayerNotAimedMe)
            {
                dogFightState = DogFightState.Chasing;
            }
        }
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

    void PlayerAimStateChanger()
    {
        if(pc.target == null)
        {
            playerAimCondition = PlayerAimCondition.PlayerNotAimedMe;
            return;
        }
        
        if(pc.target == this.gameObject)
        {
            playerAimCondition = PlayerAimCondition.PlayerAimedMe;
        }
    }

    void Chase()
    {
        Debug.Log("Chase");
        //플레이어와의 거리가 1000보다 크면 플레이어 방향으로 이동

        if(rayHitCondition == RayHitCondition.NothingHit)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);
        }


    }


    void Escape()
    {
        Debug.Log("Escape");


        if(playerAimCondition != PlayerAimCondition.PlayerAimedMe)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);

            return;
        }

        if(moveSpeed > 5.0f)
        {
            moveSpeed += 2.0f * Time.deltaTime;
            laserDamage -= 2.0f * Time.deltaTime;
        }
        else if(moveSpeed <= 5.0f)
        {
            if (laserDamage <= 0)
            {
                return;
            }

            laserDamage -= 2.0f * Time.deltaTime;
            moveSpeed -= 2.0f * Time.deltaTime;
            turnSpeed += 0.05f * Time.deltaTime;
            shield += 1.0f * Time.deltaTime;

        }
    }

    void PassBy()
    {
        //수정필요
    }


    void SpeedDown()
    {
        if(playerAimCondition == PlayerAimCondition.PlayerAimedMe)
        {
            return;
        }
        moveSpeed -= 1.0f * Time.deltaTime;
        turnSpeed += 0.05f * Time.deltaTime;

        if(playerSightCondition == PlayerSightCondition.InPlayerSight)
        {
            if(shield >= 20.0f)
            {
                return;
            }
            shield += 1.0f * Time.deltaTime;
        }
        else if(playerSightCondition == PlayerSightCondition.NotInPlayerSight)
        {
            if(laserDamage >= 20.0f)
            {
                return;
            }
            laserDamage += 1.0f * Time.deltaTime;
        }
    }

    void ShieldDown()
    {
        if(playerAimCondition == PlayerAimCondition.PlayerAimedMe)
        {
            return;
        }

        shield -= 1.0f * Time.deltaTime;

        




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
