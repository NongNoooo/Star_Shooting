using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public enum ZRotation
    {
        toRightRotation,
        toLeftRotation,
        noDir,
    }

    public enum Moving
    {
        goFront,
        goBack,
        noBoost,
    }

    public enum Stat
    {
        SpeedChageOn,
        AttackChageOn,
        shieldChangeOn,
        NothingOn,
    }

    public ZRotation zRotation;
    public Moving move;
    public Stat stat;

    public float moveSpeed;

    Vector2 screenCenter, lookInput, mouseDistance;

    public float curTime = 0;

    public float hp = 100;

    //스텟 시스템
    float statMax = 60.0f;
    [Range(0,20)] //스텟 상승 최대 최소치
    public float moveSpeedStat = 10.0f;
    float moveSpeedStatMax = 20.0f;
    [Range(0, 20)]
    public float laserDamageStat = 10.0f;
    float laserDamageStatMax = 20.0f;
    [Range(0,20)]
    public float shieldStat = 10.0f;
    float shieldStatMax = 20.0f;

    //스탯 UI
    public Image speedSliderFill;
    public Slider speedSliderUI;
    public Image laserDamageSliderFill;
    public Slider laserDamageSliderUI;
    public Image ShieldSliderFill;

    public AudioClip laserSound;
    AudioSource _as;

    public GameObject mouseCenter;
    public GameObject mouseArrow;

    public GameObject endGame;

    GameObject[] enemy;
    GameObject[] targets;

    void Start()
    {
        zRotation = ZRotation.noDir;
        stat = Stat.NothingOn;


        MousePositionInit();

        _as = GetComponent<AudioSource>();

        enemy = GameObject.FindGameObjectsWithTag("Enemy");
        targets = GameObject.FindGameObjectsWithTag("Target");
    }

    void StatDistribution()
    {
        switch (stat)
        {
            case Stat.SpeedChageOn:
                SpeedtoOther();
                speedSliderFill.color = Color.green;
                break;
            case Stat.AttackChageOn:
                AttackToOther();
                laserDamageSliderFill.color = Color.green;
                break;
            case Stat.shieldChangeOn:
                ShieldToOther();
                ShieldSliderFill.color = Color.green;
                break;
            case Stat.NothingOn:
                //스탯 변경 선택 해제시 색상을 흰색으로 변경
                StatUIColorInit();
                break;
        }
    }
    //스탯ui색상 흰색으로 변경
    void StatUIColorInit()
    {
        speedSliderFill.color = Color.white;
        laserDamageSliderFill.color = Color.white;
        ShieldSliderFill.color = Color.white;
    }
    //스텟 재분배를 위한 활성화 비활성화
    void StatChangeActivate()
    {
        if(stat == Stat.NothingOn)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                stat = Stat.SpeedChageOn;
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                stat = Stat.AttackChageOn;
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                stat = Stat.shieldChangeOn;
            }
        }
        else if (stat == Stat.SpeedChageOn)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                stat = Stat.NothingOn;
            }
        }
        else if (stat == Stat.AttackChageOn)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                stat = Stat.NothingOn;
            }
        }
        else if (stat == Stat.shieldChangeOn)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                stat = Stat.NothingOn;
            }
        }

    }
    //스피드 스탯 재분배
    void SpeedtoOther()
    {
        //스피드 스탯이 변경 활성화 되있을때     
        if(stat == Stat.SpeedChageOn)
        {
            //x키를 누르면 스피드스텟이 감소하고 공격력이 상승
            if (Input.GetKey(KeyCode.X))
            {
                //스피드 스텟이 0보다 크고
                if(moveSpeedStat >= 0)
                {
                    //레이저 데미지 스텟이 맥스치보다 낮을때만 발동함
                    if(laserDamageStat <= laserDamageStatMax)
                    {
                        moveSpeedStat -= Time.deltaTime;
                        laserDamageStat += Time.deltaTime;
                    }
                }
            }

            if (Input.GetKey(KeyCode.C))
            {
                if (moveSpeedStat >= 0)
                {
                    //레이저 데미지 스텟이 맥스치보다 낮을때만 발동함
                    if (shieldStat <= shieldStatMax)
                    {
                        moveSpeedStat -= Time.deltaTime;
                        shieldStat += Time.deltaTime;
                    }

                }
            }
        }
    }
    //공격 스탯 재분배
    void AttackToOther()
    {
        //attackChageOn상태가 아니면 작동안함
        if(stat != Stat.AttackChageOn)
        {
            return;
        }

        //레이저 데미지 스텟이 0보다 작으면 작동 x
        if (laserDamageStat <= 0)
        {
            return;
        }

        if (Input.GetKey(KeyCode.Z))
        {
            //스피드 스탯이 맥스치보다 커지면 작동x
            if(moveSpeedStat <= moveSpeedStatMax)
            {
                laserDamageStat -= Time.deltaTime;
                moveSpeedStat += Time.deltaTime;
            }
        }

        if (Input.GetKey(KeyCode.C))
        {
            if (shieldStat <= shieldStatMax)
            {
                laserDamageStat -= Time.deltaTime;
                shieldStat += Time.deltaTime;
            }

        }
    }
    //쉴드 스탯 재분배
    void ShieldToOther()
    {
        if(stat != Stat.shieldChangeOn)
        {
            return;
        }

        if (shieldStat <= 0)
        {
            return;
        }

        if (Input.GetKey(KeyCode.Z))
        {
            if (moveSpeedStat <= moveSpeedStatMax)
            {
                shieldStat -= Time.deltaTime;
                moveSpeedStat += Time.deltaTime;
            }
        }

        if (Input.GetKey(KeyCode.X))
        {
            if (laserDamageStat <= laserDamageStatMax)
            {
                shieldStat -= Time.deltaTime;
                laserDamageStat += Time.deltaTime;
            }
        }
    }


    void MousePositionInit()
    {
        //화면중앙
        screenCenter.x = Screen.width * 0.5f;
        screenCenter.y = Screen.height * 0.5f;
    }

    private void FixedUpdate()
    {
        //마우스 방향으로 기체가 바라보도록
        Turing();


        //리드커서가 마우스 위치에 위치하도록
        mouseLeadCursurPos();
    }

    public float z;
    public Transform player;
    public GameObject lead;

    //기체 x,y회전
    void Turing()
    {
        //마우스 위치 x,y를 바라봐야할 위치로 지정
        lookInput.x = Input.mousePosition.x;
        lookInput.y = Input.mousePosition.y;

        //마우스의 위치를 마우스위치 - 화면 중앙 / 화면 중앙 으로 계산해 마우스 위치를 구함
        //값은 화면중앙일땐 0 화면의 끝일땐 1로 나옴
        mouseDistance.x = (lookInput.x - screenCenter.x) / screenCenter.x;
        mouseDistance.y = (lookInput.y - screenCenter.y) / screenCenter.y;

        transform.Rotate(-mouseDistance.y * 90.0f * Time.deltaTime, mouseDistance.x * 90.0f * Time.deltaTime, 0);
    }
    //리드 마우스커서 위치를 현제 마우스위치로
    void mouseLeadCursurPos()
    {
        lead.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
    }



    void Update()
    {
        //스위치문 상태에 따라 해당 방향으로 z축 회전
        ZAxisRotation();
        //a,d키 입력에 따라 ZAxisRotation스위치문 상태 변경
        GetKeyZAxis();



        //스위치문의 상태에 따라 앞뒤로 이동
        GoMove();
        //w,s키입력에 따라 앞뒤 가속도를 받고 가속도에 따라 GoMove스위치문 상태 변경
        KeyInput();



        //레이저 발사
        Attack();


        //스텟 재분배? 변경?스위치문
        StatDistribution();
        //스텟 변경 활성화 비활성화 
        StatChangeActivate();
        //스텟 변경시 값이 0보다 작아지거나 맥스값보다 커지면 0,맥스값 으로 고정
        StatLimit();
        //스탯 슬라이더 UI표시
        StatSlider();
    }


    void StatSlider()
    {
        speedSliderUI.value = moveSpeedStat / moveSpeedStatMax;
        laserDamageSliderUI.value = laserDamageStat / laserDamageStatMax;
    }

 
    public GameObject laser;
    public GameObject laserBlaster;
    public GameObject target = null;

    //마우스 왼쪽 버튼입력으로 공격
    //autoTarget스크립트에서 Target정보 넘겨받고 laserBlast로 넘겨줌
    void Attack()
    {
        if (Input.GetMouseButtonUp(0))
        {
            _as.PlayOneShot(laserSound);
            GameObject l = Instantiate(laser);
            Destroy(l, 2.0f);
            LaserBlast lb = l.GetComponent<LaserBlast>();

            lb.damage = laserDamageStat;

            //autoTarget으로 부터 target정보를 넘겨 받아 target이 null이 아닐경우
            if (target != null)
            {
                //AutoTarget스크립트에서 전달받은 target정보를 LaserBlast로 전달
                lb.target = target;
            }

            l.transform.position = laserBlaster.transform.position;
            l.transform.rotation = laserBlaster.transform.rotation;
        }
    }

    //스텟 변경시 값이 0보다 작아지거나 맥스값보다 커지면 0,맥스값 으로 고정
    void StatLimit()
    {
        if (moveSpeedStat < 0)
        {
            moveSpeedStat = 0;
        }

        if (laserDamageStat < 0)
        {
            laserDamageStat = 0;
        }

        if (shieldStat < 0)
        {
            shieldStat = 0;
        }

        if (moveSpeedStat > moveSpeedStatMax)
        {
            moveSpeedStat = moveSpeedStatMax;
        }

        if (laserDamageStat > laserDamageStatMax)
        {
            laserDamageStat = laserDamageStatMax;
        }

        if (shieldStat > shieldStatMax)
        {
            shieldStat = shieldStatMax;
        }
    }

    //스위치문 상태에 따라 기체의 앞뒤로 가속
    void GoMove()
    {
        switch (move)
        {
            case Moving.goFront:
                Go();
                break;
            case Moving.goBack:
                Go();
                break;
        }
    }

    void KeyInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            if(moveSpeed <= moveSpeedStat)
            {
                moveSpeed += Time.deltaTime;
            }
            else if(moveSpeed >= moveSpeedStat)
            {
                moveSpeed = moveSpeedStat;
            }
        }

        if (Input.GetKey(KeyCode.S))
        {
            if(moveSpeed >= -moveSpeedStat)
            {
                moveSpeed -= Time.deltaTime;
            }
            else if(moveSpeed <= -moveSpeedStat)
            {
                moveSpeed = -moveSpeedStat;
            }
        }

        if (moveSpeed > 0)
        {
            move = Moving.goFront;
        }
        else if (moveSpeed < 0)
        {
            move = Moving.goBack;
        }
    }
    //위에서 받은 가속도에 따라 이동
    //s키를 누적시키면 movespeed값이 -가 됨으로 뒤로감
    void Go()
    {
        transform.position += transform.forward * moveSpeed * 300.0f * Time.deltaTime;
    }



    //a,d키를 이용해 z축 회전을 위한 스위치문
    void ZAxisRotation()
    {
        switch (zRotation)
        {
            case ZRotation.toRightRotation:
                GoZAxisRotation();
                return;
            case ZRotation.toLeftRotation:
                GoZAxisRotation();
                return;
            case ZRotation.noDir:
                return;
        }
    }
    //a,d키의 입력에 따라 스위치문 상태 변경
    void GetKeyZAxis()
    {
        if (Input.GetKey(KeyCode.D))
        {
            zRotation = ZRotation.toRightRotation;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            zRotation = ZRotation.noDir;
        }

        if (Input.GetKey(KeyCode.A))
        {
            zRotation = ZRotation.toLeftRotation;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            zRotation = ZRotation.noDir;
        }
    }
    //스위치문의 상태에 따라 z축 회전
    void GoZAxisRotation()
    {
        if (zRotation == ZRotation.toRightRotation)
        {
            transform.Rotate(0, 0, -100 * Time.deltaTime);
        }

        if (zRotation == ZRotation.toLeftRotation)
        {
            transform.Rotate(0, 0, 100 * Time.deltaTime);
        }
    }


    //데미지 처리
    public void Damaged(float damage)
    {
        hp -= damage;

        if(hp > 0)
        {
            return;
        }

        Debug.Log("HP 0");

        for (int i = 0; i < enemy.Length; i++)
        {
            Destroy(enemy[i]);
            Destroy(targets[i]);
        }

        endGame.SetActive(true);
        gameObject.SetActive(false);
    }
}
