using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public enum GoHorizontal
    {
        goRight,
        goLeft,
        noDir,
    }

    public enum GoVertical
    {
        goDown,
        goUp,
        noDir,
    }

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

    public enum ZRotationState
    {
        Up,
        UpRight,
        UpLeft,
        Down,
        DownRight,
        DownLeft,
        Right,
        Left,
    }


    public GoHorizontal goHorizontal;
    public GoVertical goVertical;
    public ZRotation zRotation;
    public Moving move;
    public ZRotationState rotationState;

    float mh;
    float mv;

    public float xSpeed;
    public float ySpeed;
    public float zSpeed;

    public float moveSpeed;

    Vector2 screenCenter, lookInput, mouseDistance;

    float mOriPosX;
    float mOriPosY;

    //???? ???? ???? ??????
    float maxTurningSpeedLow = 5.0f;
    float maxTurningSpeedHigh = 10.0f;

    //?????????? ???????? ?????? ?????? ??????
    float goDirValue = 10.0f;

    public float curTime = 0;


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
    public Image laserDamageSliderFill;
    public Image ShieldSliderFill;

    public AudioClip laserSound;
    AudioSource _as;

    //이동속도에 따른 방향 회전 속도 증가 감소
    float turningSpeed;

    public enum Stat
    {
        SpeedChageOn,
        AttackChageOn,
        shieldChangeOn,
        NothingOn,
    }

    public Stat stat;

    void Start()
    {
        //???? ?????? ?????? ???? ??????????
        //Cursor.lockState = CursorLockMode.Locked;


        goHorizontal = GoHorizontal.noDir;
        goVertical = GoVertical.noDir;
        zRotation = ZRotation.noDir;
        rotationState = ZRotationState.Up;
        stat = Stat.NothingOn;


        MousePositionInit();

        _as = GetComponent<AudioSource>();
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
        //???? ?????? ?????? ???????? ??????
        //??????
        mOriPosX = Input.mousePosition.x;
        mOriPosY = Input.mousePosition.y;

        //화면중앙
        screenCenter.x = Screen.width * 0.5f;
        screenCenter.y = Screen.height * 0.5f;

    }

    private void FixedUpdate()
    {
        //기체 x,y축 회전
        Turing();


        //리드커서가 마우스 위치에 위치하도록
        mouseLeadCursurPos();
    }

    Vector3 toRotation;
    public float z;
    public Transform player;

    //기체 x,y회전
    void Turing()
    {
        toRotation.y += 5 * xSpeed * Time.deltaTime;
        toRotation.x += 5 * ySpeed * Time.deltaTime;



        //마우스 위치 x,y를 바라봐야할 위치로 지정
        lookInput.x = Input.mousePosition.x;
        lookInput.y = Input.mousePosition.y;

        //마우스의 위치를 마우스위치 - 화면 중앙 / 화면 중앙으로 백분률?로 받음
        //값은 화면중앙일땐 0 화면의 끝일땐 1로 나오긴함
        mouseDistance.x = (lookInput.x - screenCenter.x) / screenCenter.x;
        mouseDistance.y = (lookInput.y - screenCenter.y) / screenCenter.y;


        //transform.Rotate(-mouseDistance.y * 90.0f * Time.deltaTime, mouseDistance.x * 90.0f * Time.deltaTime, transform.rotation.z);
        if (xSpeed > 0 || xSpeed < 0 || ySpeed > 0 || ySpeed < 0)
        {
            transform.Rotate(-mouseDistance.y * 90.0f * Time.deltaTime, mouseDistance.x * 90.0f * Time.deltaTime, 0);

        }
    }
    //리드 마우스커서 위치를 현제 마우스위치로
    void mouseLeadCursurPos()
    {
        lead.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
    }

    public GameObject zRotObj;
    public GameObject camPos;
    public GameObject lead;
    public GameObject front;


    void Update()
    {
        //?????? ???? ???????? ??????
        MousePosition();



        //?????? ?????? ???? ???????????? ???????? ???????? ??????
        MouseHorizontalPosCheck();
        MouseVerticalPosCheck();



        //?????? ?????????? ???? ???? ????
        HorizontalRotation();
        VeticalRotation();

        

        ZRot();
        //z?? ?????? ???? ZRot???????? ???? ????
        degree();



        //스위치문 상태에 따라 해당 방향으로 z축 회전
        ZAxisRotation();
        //a,d키 입력에 따라 ZAxisRotation스위치문 상태 변경
        GetKeyZAxis();



        //스위치문의 상태에 따라 앞뒤로 이동
        GoMove();
        //w,s키입력에 따라 앞뒤 가속도를 받고 가속도에 따라 GoMove스위치문 상태 변경
        KeyInput();



        //??????????
        SpeedClamp();



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
        speedSliderFill.fillAmount = moveSpeedStat / moveSpeedStatMax;
        laserDamageSliderFill.fillAmount = laserDamageStat / laserDamageStatMax;
        ShieldSliderFill.fillAmount = shieldStat / shieldStatMax;
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
            Destroy(l, 1.0f);
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
                /* case Moving.noBoost:
                     LoseSpeed();
                     break;*/
        }
    }
    //w키로 앞으로 가속
    //s키로 뒤로 가속
    //우주 공간이라 저항이 없어 감속이 일어나지 않는다고 생각해서
    //키입력을 멈췄을때 해당 위치에서 멈추는게 아니라 가속도를 얻는것을 멈춤
    //가속도가 0 이상일땐 앞으로
    //0이하일땐 뒤로 이동
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
        transform.position += transform.forward * moveSpeed * 100.0f * Time.deltaTime;
    }



    //?????? ?????? ???????? ??????
    void MousePosition()
    {
        //?????? ???????? ???????????? ?????? ?????? ???????? ????
        //?????????? mh, mv?? 0???? ???????? ???? ????
        mh = Input.mousePosition.x - mOriPosX;
        mv = Input.mousePosition.y - mOriPosY;

        mousPosLock();

        /*        Debug.Log(goHorizontal);
                Debug.Log(goVertical);
        */
    }
    //?????? ?????? ???? ??????
    void mousPosLock()
    {
        //?????? ???????? 300, -300???? ???????? ????

        mh = Mathf.Clamp(mh, -300.0f, 300.0f);
        mv = Mathf.Clamp(mv, -300.0f, 300.0f);
    }


    //?????? ?????? ???? ???? ???? ??????
    void MouseHorizontalPosCheck()
    {
        if (mh > -goDirValue && mh < goDirValue)
        {
            goHorizontal = GoHorizontal.noDir;
        }
        else if (mh > goDirValue)
        {
            //mh?? goDirValu(100)???? ?????? ?????????? ????
            goHorizontal = GoHorizontal.goRight;
        }
        else if (mh < -goDirValue)
        {
            //mh?? -goDirValu(100)???? ???????? ???????? ????
            goHorizontal = GoHorizontal.goLeft;
        }
    }
    void MouseVerticalPosCheck()
    {
        if (mv > -goDirValue && mv < goDirValue)
        {
            goVertical = GoVertical.noDir;
        }
        else if (mv < goDirValue)
        {
            goVertical = GoVertical.goDown;
        }
        else if (mv > -goDirValue)
        {
            goVertical = GoVertical.goUp;
        }
    }


    //???? ???? ?????? ???? ???? ?????? ???? ???? ??????
    void VeticalRotation()
    {
        switch (goVertical)
        {
            case GoVertical.goUp:
                GoUp();
                return;
            case GoVertical.goDown:
                GoDown();
                return;
            case GoVertical.noDir:
                StopVeticalRotation();
                return;
        }
    }
    void GoUp()
    {
        //???? ?????? ???? ?????? ?????? ??
        ySpeed += 2 * Time.deltaTime;
    }
    void GoDown()
    {
        //???? ?????? ???? ?????? ?????? ??
        ySpeed -= 2 * Time.deltaTime;
    }
    void StopVeticalRotation()
    {
        //???????? ???????? ???? ???? ???? ?????? 0???? ??????
        //?? ???? ???????? ???? ????
        /*  if(ySpeed < 0)
          {
              //ySpeed += 10 * Time.deltaTime;
              ySpeed = Mathf.Lerp(ySpeed, 0.0f, 1.0f);
          }
          else if(ySpeed > 0)
          {
              ySpeed = Mathf.Lerp(ySpeed, 0.0f, 1.0f);
          }*/
        ySpeed = Mathf.Lerp(ySpeed, 0.0f, 1.0f);
    }

    void HorizontalRotation()
    {
        switch (goHorizontal)
        {
            case GoHorizontal.goRight:
                GoRight();
                return;
            case GoHorizontal.goLeft:
                GoLeft();
                return;
            case GoHorizontal.noDir:
                StopHorizontalRotation();
                return;
        }
    }
    void GoRight()
    {
        //???? ?????? ???? ?????? ??
        xSpeed += 2 * Time.deltaTime;

    }
    void GoLeft()
    {
        //?????????? ???? ?????? ??
        xSpeed -= 2 * Time.deltaTime;
    }
    void StopHorizontalRotation()
    {
        //???????? ???? ???????? ???????? ?????????? 0???? ??????
        //?? ???? ???????? ???? ????
        xSpeed = Mathf.Lerp(xSpeed, 0.0f, 1.0f);

        //zSpeed = Mathf.Lerp(zSpeed, 0.0f, 1.0f);
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
    //z축 회전도 앞뒤 이동 공식처럼 가속도를 얻게되면 멈추지않고 계속 회전시키려고했는데
    //멀미때문에 키입력을 회전하지않도록 제작
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
            //z -= 100 * Time.deltaTime;
            transform.Rotate(0, 0, -100 * Time.deltaTime);
        }

        if (zRotation == ZRotation.toLeftRotation)
        {
            transform.Rotate(0, 0, 100 * Time.deltaTime);
        }
    }


    //???????????
    //현제 기체의 회전 상태
    void ZRot()
    {
        switch (rotationState)
        {
            case ZRotationState.Up:
                break;
            case ZRotationState.Down:
                break;
            case ZRotationState.Right:
                break;
            case ZRotationState.Left:
                break;
            case ZRotationState.UpRight:
                break;
            case ZRotationState.UpLeft:
                break;
            case ZRotationState.DownRight:
                break;
            case ZRotationState.DownLeft:
                break;
        }
    }
    //현제 기체의 z각도에 따라 스위치문 상태 변경
    void degree()
    {
        float angle = transform.eulerAngles.z;

        if (angle >= 0 && angle <= 15.0f)
        {
            //Debug.Log("Upside");
            rotationState = ZRotationState.Up;
        }
        if (angle < 360.0f && angle >= 345.0f)
        {
            //Debug.Log("Upside");
            rotationState = ZRotationState.Up;
        }
        else if (angle > 15.0f && angle <= 75.0f)
        {
            //Debug.Log("Up,left");
            rotationState = ZRotationState.UpLeft;
        }
        else if (angle > 75.0f && angle <= 105.0f)
        {
            //Debug.Log("Left");
            rotationState = ZRotationState.Left;
        }
        else if (angle > 105.0f && angle <= 165.0f)
        {
            //Debug.Log("left,down");
            rotationState = ZRotationState.DownLeft;
        }
        else if (angle > 165.0f && angle <= 195.0f)
        {
            //Debug.Log("down");
            rotationState = ZRotationState.Down;
        }
        else if (angle > 195.0f && angle <= 255.0f)
        {
            //Debug.Log("down,right");
            rotationState = ZRotationState.DownRight;
        }
        else if (angle > 255.0f && angle <= 285.0f)
        {
            //Debug.Log("right");
            rotationState = ZRotationState.Right;
        }
        else if (angle > 285.0f && angle <= 345.0f)
        {
            //Debug.Log("up,right ");
            rotationState = ZRotationState.UpRight;
        }
    }

    //???????????
    //zRot상태에 따른 최대 최소속도 조절
    void SpeedClamp()
    {
        //???? ????
        if (goHorizontal == GoHorizontal.noDir)
        {
            //???? ???? ?????? ????????
            if (goVertical == GoVertical.goUp)
            {
                //???? z?? ?????? ???? ????
                if (rotationState == ZRotationState.Up)
                {
                    ySpeed = Mathf.Clamp(ySpeed, -maxTurningSpeedHigh, maxTurningSpeedHigh);
                    xSpeed = Mathf.Clamp(xSpeed, -maxTurningSpeedLow, maxTurningSpeedLow);
                }
                else if (rotationState == ZRotationState.Down)
                {
                    ySpeed = Mathf.Clamp(ySpeed, -maxTurningSpeedHigh, maxTurningSpeedHigh);
                    xSpeed = Mathf.Clamp(xSpeed, -maxTurningSpeedLow, maxTurningSpeedLow);
                }
                else
                {
                    ySpeed = Mathf.Clamp(ySpeed, -maxTurningSpeedLow, maxTurningSpeedLow);
                    xSpeed = Mathf.Clamp(xSpeed, -maxTurningSpeedLow, maxTurningSpeedLow);
                }
            }

        }
        else if (goHorizontal == GoHorizontal.goRight)
        {
            if (goVertical == GoVertical.noDir)
            {
                if (rotationState == ZRotationState.UpRight)
                {
                    ySpeed = Mathf.Clamp(ySpeed, -maxTurningSpeedLow, maxTurningSpeedLow);
                    xSpeed = Mathf.Clamp(xSpeed, -maxTurningSpeedHigh, maxTurningSpeedHigh);
                }
                else if (rotationState == ZRotationState.Right)
                {
                    ySpeed = Mathf.Clamp(ySpeed, -maxTurningSpeedLow, maxTurningSpeedLow);
                    xSpeed = Mathf.Clamp(xSpeed, -maxTurningSpeedHigh, maxTurningSpeedHigh);
                }
                else
                {
                    ySpeed = Mathf.Clamp(ySpeed, -maxTurningSpeedLow, maxTurningSpeedLow);
                    xSpeed = Mathf.Clamp(xSpeed, -maxTurningSpeedLow, maxTurningSpeedLow);
                }
            }
        }
    }
 
}
