using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum DogFightState
    {
        Chasing,
        Chased,
        ChaseAgain,
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


    [Range(0, 20.0f)]
    public float moveSpeed = 10.0f;
    [Range(0.2f, 2.0f)]
    public float turnSpeed = 0.2f;
    [Range(0, 20.0f)]
    public float laserDamage = 10.0f;
    [Range(0, 20.0f)]
    public float shield = 10.0f;

    //자신(적) 시야 범위
    float sightAngle = 20.0f;
    //자신(적) 공격 범위
    float attackSightAngle = 90.0f;
    //플레이어의 시야범위
    float playerSightAngle = 50.0f;

    GameObject player;
    PlayerController pc;

    //공격쿨타임 시간누적
    public float attackCurTime = 0;
    public GameObject laserObj;
    [System.NonSerialized]
    public GameObject target;

    //사망시 폭발이펙트
    public GameObject explosion;

    //플레이어와 자신의 차로 구한 방향
    Vector3 dir;

    float hp = 0;
    float maxHp = 100;

    GameObject g;
    GameManager gm;

    public AudioClip laserSound;

    AudioSource _as;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        player = p.transform.Find("PlayerTargetPos").gameObject;
        pc = p.GetComponent<PlayerController>();

        dogFightState = DogFightState.Chasing;

        g = GameObject.FindGameObjectWithTag("GameManager");
        gm = g.GetComponent<GameManager>();

        _as = GetComponent<AudioSource>();
    }


    void Update()
    {
        //플레이어로의 방향 찾음
        dir = (player.transform.position - transform.position).normalized;


        Move();

        //공격 쿨타임
        LaserFireCount();
        LaserFire();



        //레이를 이용해 전방에 물체가 있는지 판단해 아래 스위치문 변경
        UseRayCheckDistanceChanger();
        RayDistanceState();


        //자신(적)의 시야내에 플레이어가 있는지 판단해 아래 스위치문 변경
        EnemySightStateChanger();
        EnemySightState();

        //플레이어의 시야내에 자신(적)이 있는지 판단해 아래 스위치문 변경
        PlayerSightStateChanger();
        PlayerSightState();

        //플레이어가 자신(적)을 조준하고 있는지 판단해 아래 스위치문 변경
        PlayerAimStateChanger();
        PlayerAimState();



        //위 3개의 스위치 문의 상태에 따라 자신의 움직임을 결정하는 스위치문
        DogFightStateChanger();
        DogFight();
    }

    //이동
    void Move()
    {
        transform.position += transform.forward * moveSpeed * 300.0f * Time.deltaTime;
    }

    //공격 쿨타임
    void LaserFireCount()
    {
        //attackCurTime이 1보다 크면 리턴
        if (attackCurTime >= 1.0f)
        {
            return;
        }
        attackCurTime += Time.deltaTime;
    }
    //공격
    void LaserFire()
    {
        if (target == null)
        {
            return;
        }

        if (attackCurTime < 1.0f)
        {
            return;
        }

        _as.PlayOneShot(laserSound);
        GameObject l = Instantiate(laserObj);
        Destroy(l, 8.0f);
        l.transform.position = transform.position;

        //Debug.Log("적 공격");
        attackCurTime = 0.0f;

        EnemyLaserBlast elb = l.GetComponent<EnemyLaserBlast>();
        elb.target = target;
        elb.laserDamage = laserDamage;
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
            if (Physics.Raycast(transform.position, transform.forward, out hit, 500.0f, mask))
            {
                rayHitCondition = RayHitCondition.SomethingHit;
            }
            else
            {
                rayHitCondition = RayHitCondition.NothingHit;
            }

            return;
        }


        //플레이어와 마주보고있는 상황이 아닐경우
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1000.0f, mask))
        {
            //Debug.Log("1000 + " + hit.transform.name);
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
                break;
            case RayHitCondition.SomethingHit:
                Avoid();
                break;
        }
    }
    //위 스위치문에 따라 레이에 물체가 감지되면 회피
    void Avoid()
    {
        Debug.Log("Avoid");
        float xRot = 2.0f * Time.deltaTime;
        float zRot = 2.0f * Time.deltaTime;

        transform.rotation *= Quaternion.Euler(0, 0, 10 * Time.deltaTime);

        transform.position += transform.up * 30 * Time.deltaTime;
    }


    //자신의 시야범위 내에 플레이어가 있는지 없는지 확인
    void EnemySightStateChanger()
    {
        //자신과 플레이어간의 각도 구하기
        //자신의 forword방향과 dir(플레이어와 자신의 백터 차)각도
        float dot = Vector3.Dot(transform.forward, dir);

        //위에서 계산한 dot을 각도로 변경
        //acos dot만 계산하면 라디안이 나옴 Rad2Deg를 곱해서 각도로 받음
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

        if (theta <= attackSightAngle)
        {
            target = player;

        }
        else
        {
            target = null;
        }
    }
    void EnemySightState()
    {
        switch (enemySightCondition)
        {
            case EnemySightCondition.LookAtPlayer:
                break;
            case EnemySightCondition.NotLookAtPlayer:
                break;

        }
    }

    //플레이어의 시야범위 내에 자신(적)이 있는지 없는지 확인
    void PlayerSightStateChanger()
    {
        float dot = Vector3.Dot(player.transform.forward, (transform.position - player.transform.position).normalized);

        float theta = Mathf.Acos(dot) * Mathf.Rad2Deg;

        if (theta <= playerSightAngle)
        {
            playerSightCondition = PlayerSightCondition.InPlayerSight;
        }
        else
        {
            playerSightCondition = PlayerSightCondition.NotInPlayerSight;
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


    public float curTime = 0f;
    float dodgeTime = 3.0f;
    //플레이어가 자신을 조준하고있는지 확인
    void PlayerAimStateChanger()
    {
        if (pc.target == null)
        {
            playerAimCondition = PlayerAimCondition.PlayerNotAimedMe;
            return;
        }

        if (pc.target == this.gameObject)
        {
            playerAimCondition = PlayerAimCondition.PlayerAimedMe;
        }
    }
    void PlayerAimState()
    {
        switch (playerAimCondition)
        {
            case PlayerAimCondition.PlayerAimedMe:
                Dodge();
                break;
            case PlayerAimCondition.PlayerNotAimedMe:
                break;
        }
    }
    //플레이어가 자신(적)을 타게팅시 회피
    void Dodge()
    {
        float dis = Vector3.Distance(transform.position, player.transform.position);

        if (dis < 800)
        {
            moveSpeed *= -1;
        }

        if (curTime < dodgeTime)
        {
            curTime += Time.deltaTime;
        }

        float dot = Vector3.Dot(transform.forward, dir);

        //위에서 계산한 dot을
        //내적을 이용한 각 계산하기
        //thetha = cos^-1( a dot b / |a||b|)
        float theta = Mathf.Acos(dot) * Mathf.Rad2Deg;

        //외적으로 플레이어가 자신(적)의 오른쪽에 있는지 왼쪽에 있는지 구분
        Vector3 cross = Vector3.Cross(transform.forward, dir);


        if(theta < 30)
        {
            return;
        }

        if(theta < 100)
        {
            //Debug.Log(cross.y);
            //플레이어가 자신(적)의 기준 오른쪽에 위치해있으면
            if (cross.y > 0.0f)
            {
                DodgeDownLeft();
            }
            //플레이어가 자신(적)의 기준 왼쪽에 위치해있으면
            else if(cross.y < 0.0f)
            {
                DodgeUpRight();
            }
        }
        else if(theta >= 100)
        {
            if(curTime >= dodgeTime)
            {
                int a = Random.Range(0, 3);
                curTime = 0;

                if(a == 0)
                {
                    DodgeUpRight();
                }
                else if(a == 1)
                {
                    DodgeUpLeft();
                }
                else if(a == 2)
                {
                    DodgeDownRight();
                }
                else if(a == 3)
                {
                    DodgeDownLeft();
                }
            }

            //Debug.Log(dis);
        }
    }

    void DodgeUpRight()
    {
        //위로 이동
        transform.position += transform.up * 300 * Time.deltaTime;
        //위로 x축 회전 오른쪽으로 z축 회전
        transform.rotation *= Quaternion.Euler(-40 * Time.deltaTime, 0, 40 * Time.deltaTime);
    }
    void DodgeUpLeft()
    {
        //위로 이동
        transform.position += transform.up * 300 * Time.deltaTime;
        //위로 x축 회전 왼쪽 z축 회전
        transform.rotation *= Quaternion.Euler(-40 * Time.deltaTime, 0, -40 * Time.deltaTime);
    }
    void DodgeDownRight()
    {
        //아래로 이동
        transform.position += -transform.up * 300 * Time.deltaTime;
        //아래로 x축 회전 오른쪽으로 z축 회전
        transform.rotation *= Quaternion.Euler(40 * Time.deltaTime, 0, 40 * Time.deltaTime);
    }
    void DodgeDownLeft()
    {
        //아래로 이동
        transform.position += -transform.up * 300 * Time.deltaTime;
        //아래로 x축 회전 왼쪽으로 z축 회전
        transform.rotation *= Quaternion.Euler(40 * Time.deltaTime, 0, -40 * Time.deltaTime);
    }
    

    //위 3개의 스위치문에 따라 쫒는중,쫒기는중,면대면으로 상태변경
    //상태에 따라 움직임 설정
    void DogFightStateChanger()
    {
        //플레이어를 바라보고 있음
        if (enemySightCondition == EnemySightCondition.LookAtPlayer)
        {
            //플레이어가 자신을 바라보고있지 않을때 
            if (playerSightCondition == PlayerSightCondition.NotInPlayerSight)
            {
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
            else if(playerSightCondition == PlayerSightCondition.NotInPlayerSight)
            {
                dogFightState = DogFightState.ChaseAgain;
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
            case DogFightState.ChaseAgain:
                ChaseAgainToPlayer();
                break;
            case DogFightState.FaceToFace:
                PassBy();
                break;
            case DogFightState.DoNothing:
                break;
        }
    }


    void Chase()
    {
        Debug.Log("Chase");

        if(rayHitCondition == RayHitCondition.NothingHit)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);
        }

        moveSpeed += 2.0f * Time.deltaTime;
        turnSpeed = 0.2f;

        if(shield > 0)
        {
            shield -= 1.0f * Time.deltaTime;

            if (laserDamage >= 20.0f)
            {
                return;
            }

            laserDamage += 1.0f * Time.deltaTime;
        }
    }

    float escCurTime = 0;
    bool up = true;
    bool down = false;
    void Escape()
    {
        Debug.Log("Escape");
        escCurTime += Time.deltaTime;

        if(up == true)
        {
            transform.rotation *= Quaternion.Euler(-10 * Time.deltaTime, 0, 0);
        }
        else if(down == true)
        {   
            transform.rotation *= Quaternion.Euler(10 * Time.deltaTime, 0, 0);
        }

        if(escCurTime > 10.0f)
        {
            up = !up;
            down = !down;
            escCurTime = 0;
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);

    }

    void ChaseAgainToPlayer()
    {
        Debug.Log("ChaseAgainToPlayer");
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);

        if(moveSpeed < 10.0f)
        {
            moveSpeed += 3.0f * Time.deltaTime;
        }
        
        //turnSpeed += 0.1f * Time.deltaTime;

        if(laserDamage >= 20.0f)
        {
            return;
        }
        //레이저 데미지가 20보다 작으면 레이저 데미지 올림
        laserDamage += 1.0f * Time.deltaTime;
    }

    void PassBy()
    {
        if(moveSpeed > 5.0f)
        {
            moveSpeed -= 2.0f * Time.deltaTime;
            laserDamage += 1.0f * Time.deltaTime;
            shield += 1.0f * Time.deltaTime;
        }
    }

    //데미지 처리
    public void Damaged(float laserDamage)
    {
        if(shield > 0)
        {
            shield -= laserDamage;
        }

        if(shield <= 0)
        {
            hp -= laserDamage;
        }

        if (hp > 0)
        {
            return;
        }

        GameObject ex = Instantiate(explosion, transform.position, transform.rotation);
        gm.EnemyDie();
        Destroy(this.gameObject);
    }
}
