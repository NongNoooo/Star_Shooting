using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
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

    float mOriPosX;
    float mOriPosY;

    //방향 전환 속도 맥스값
    float maxTurningSpeedLow = 5.0f;
    float maxTurningSpeedHigh = 10.0f;

    //방향전환을 입력받기 시작할 마우스 위치값
    float goDirValue = 100.0f;

    void Start()
    {
        //게임 시작시 마우스 커서 안보이도록
        //Cursor.lockState = CursorLockMode.Locked;


        goHorizontal = GoHorizontal.noDir;
        goVertical = GoVertical.noDir;
        zRotation = ZRotation.noDir;
        rotationState = ZRotationState.Up;


        MousePositionInit();
    }

    void MousePositionInit()
    {
        //게임 시작시 마우스 위치값을 받아옴
        mOriPosX = Input.mousePosition.x;
        mOriPosY = Input.mousePosition.y;
    }

    private void FixedUpdate()
    {
        //회전최대 각도
        TurningLock();


        //회전하는 코드
        Turing();
    }

    Vector2 toRotation;

    //마우스 위치에 따라 상승한 가속도에 따라 회전
    void Turing()
    {
        //기체가 뒤집어질경우 조작키 연속성을 위해 위아래 속도 입력값 +- 변경
        if (rotationState == ZRotationState.Up || rotationState == ZRotationState.UpRight || rotationState == ZRotationState.UpLeft || rotationState == ZRotationState.Right)
        {
            toRotation.y += 5 * xSpeed * Time.deltaTime;
            toRotation.x += 5 * ySpeed * Time.deltaTime;
        }
        if (rotationState == ZRotationState.Down || rotationState == ZRotationState.DownRight || rotationState == ZRotationState.DownLeft || rotationState == ZRotationState.Left)
        {
            toRotation.y -= 5 * xSpeed * Time.deltaTime;
            toRotation.x -= 5 * ySpeed * Time.deltaTime;
        }


        transform.eulerAngles = new Vector3(-toRotation.x, toRotation.y, transform.eulerAngles.z);


        //각도를 x로 지정하면 정상적으로 회전함




        //정상적으로 회전함
        /* transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(dir), 1.0f);

         if (transform.rotation.eulerAngles.x < -120.0f)
         {
             transform.eulerAngles = new Vector3(-120.0f, transform.rotation.y, transform.rotation.z);
         }*/
    }
    //x,y축 회전 각도 제한
    void TurningLock()
    {
        //회전 각도 제한
        //toRotation.x = Mathf.Clamp(toRotation.x, -120.0f, 120.0f);
        if(toRotation.x < -120.0f)
        {
            toRotation.x = -120.0f;
        }

        toRotation.y = Mathf.Clamp(toRotation.y, -120.0f, 120.0f);
        //toRotation.z = Mathf.Clamp(toRotation.z, -30.0f, 30.0f);
    }

    void Update()
    {
        //마우스 위치 받아오는 메서드
        MousePosition();

        //마우스 위치에 따라 어느방향으로 회전할지 결정하는 메서드
        MouseHorizontalPosCheck();
        MouseVerticalPosCheck();

        //결정된 회전방향에 따라 회전 작동
        HorizontalRotation();
        VeticalRotation();

        //기체 z축 회전 상태에 따라 상태 변경
        ZRot();
        //z축 방향에 따라 ZRot스위치문 상태 변경
        degree();

        //기체 Z축 회전 스위치문
        ZAxisRotation();
        //방향키 a,d 입력에 따라
        //스위치문 상태변경
        GetKeyZAxis();

        //앞,뒤 전진 스위치문
        GoMove();
        //가속되는 방향에 따라 GoMove스위치문 상태 변경
        KeyInput();

        SpeedClamp();
    }

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

    //w또는 s를 누르고 있는동안만 해당 방향으로 가속
    //가속이 있는 방향(앞 또는 뒤)에 따라 move상태 스위치
    void KeyInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            moveSpeed += Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            moveSpeed -= Time.deltaTime;
        }

        if (moveSpeed > 0)
        {
            move = Moving.goFront;
        }
        else if(moveSpeed < 0)
        {
            move = Moving.goBack;
        }
    }
    //이동 코드
    void Go()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    //마우스 위치값 받아오는 메서드
    void MousePosition()
    {
        //마우스 위치값에 게임시작시에 받아온 마우스 위치값을 빼서
        //게임시작시 mh, mv가 0으로 시작할수 있게 만듬
        mh = Input.mousePosition.x - mOriPosX;
        mv = Input.mousePosition.y - mOriPosY;

        mousPosLock();

        Debug.Log(goHorizontal);
        Debug.Log(goVertical);
    }
    //마우스 입력값 제한 메서드
    void mousPosLock()
    {
        //마우스 입력값이 300, -300보다 못커지게 만듬

        mh = Mathf.Clamp(mh, -300.0f, 300.0f);
        mv = Mathf.Clamp(mv, -300.0f, 300.0f);
    }


    //마우스 위치에 따라 현재 상태 스위치
    void MouseHorizontalPosCheck()
    {
        if(mh > -goDirValue && mh < goDirValue)
        {
            goHorizontal = GoHorizontal.noDir;
        }
        else if (mh > goDirValue)
        {
            //mh가 goDirValu(100)보다 커지면 오른쪽으로 회전
            goHorizontal = GoHorizontal.goRight;
        }
        else if (mh < -goDirValue)
        {
            //mh가 -goDirValu(100)보다 작아지면 왼쪽으로 회전
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


    //상하 회전 스위치 문과 현재 상태에 따른 회전 메서드
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
        //회전 방향에 맞게 회전에 가속도 줌
        ySpeed += 2 * Time.deltaTime;
    }
    void GoDown()
    {
        //회전 방향에 맞게 회전에 가속도 줌
        ySpeed -= 2 * Time.deltaTime;
    }
    void StopVeticalRotation()
    {
        //마우스가 중앙으로 오면 상하 회전 가속을 0으로 만들어
        //더 이상 회전하지 않게 만듬
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
        //회전 방향에 맞게 가속도 줌
        xSpeed += 2 * Time.deltaTime;

    }
    void GoLeft()
    {
        //회전방향에 맞게 가속도 줌
        xSpeed -= 2 * Time.deltaTime;
    }
    void StopHorizontalRotation()
    {
        //마우스가 화면 중앙으로 오게되면 회전가속을 0으로 만들어
        //더 이상 회전하지 않게 만듬
        xSpeed = Mathf.Lerp(xSpeed, 0.0f, 1.0f);

        //zSpeed = Mathf.Lerp(zSpeed, 0.0f, 1.0f);
    }

    //z축 스위치문
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
    //a,d키에 따라 위 메서드의 스위치 문의 상태를 변경시키는 코드
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
    //스위치 문의 상태에 따라 기체를 z축으로 회전
    void GoZAxisRotation()
    {
        Vector3 tr = transform.eulerAngles;

        if (zRotation == ZRotation.toRightRotation)
        {
            Quaternion rot = Quaternion.Euler(0f, 0f, -100 * Time.deltaTime);

            transform.rotation *= rot;

            /*Quaternion rot = Quaternion.Euler(0f, 0f, -100f);

            Quaternion turn = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime);

            transform.rotation = turn;*/
        }

        if(zRotation == ZRotation.toLeftRotation)
        {
            Quaternion rot = Quaternion.Euler(0f, 0f, 100f * Time.deltaTime);

            transform.rotation *= rot;
        }
    }

    //회전 가속도 최대속도 제한
    void SpeedLowLimit(ref float speed)
    {
        speed = Mathf.Clamp(speed, -maxTurningSpeedLow, maxTurningSpeedLow);
    }
    void SpeedHighLimit(float speed)
    {
        speed = Mathf.Clamp(speed, -maxTurningSpeedHigh, maxTurningSpeedHigh);
    }


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

    void degree()
    {
        float angle = transform.eulerAngles.z;

        Debug.Log(angle);

        if(angle >= 0 && angle <= 15.0f)
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
        else if(angle > 195.0f && angle <= 255.0f)
        {
            //Debug.Log("down,right");
            rotationState = ZRotationState.DownRight;
        }
        else if(angle > 255.0f && angle <= 285.0f)
        {
            //Debug.Log("right");
            rotationState = ZRotationState.Right;
        }
        else if(angle > 285.0f && angle <= 345.0f)
        {
            //Debug.Log("up,right ");
            rotationState = ZRotationState.UpRight;
        }
    }


    //전지 또는 후진일때
    //degree의 상태가 up또는 down

    void SpeedClamp()
    {
        if(goHorizontal == GoHorizontal.goRight && goVertical == GoVertical.noDir)
        {
            if (rotationState != ZRotationState.Right)
            {
                xSpeed = Mathf.Clamp(xSpeed, -maxTurningSpeedLow, maxTurningSpeedLow);
            }
            else if (rotationState == ZRotationState.Right)
            {
                xSpeed = Mathf.Clamp(xSpeed, -maxTurningSpeedHigh, maxTurningSpeedHigh);
            }
        }
        else if (goHorizontal == GoHorizontal.goRight && goVertical == GoVertical.goUp)
        {
            if(rotationState != ZRotationState.UpRight)
            {
                xSpeed = Mathf.Clamp(xSpeed, -maxTurningSpeedLow, maxTurningSpeedLow);
            }
            else if(rotationState == ZRotationState.UpRight)
            {
                xSpeed = Mathf.Clamp(xSpeed, -maxTurningSpeedHigh, maxTurningSpeedHigh);
            }
        }
    }
}
