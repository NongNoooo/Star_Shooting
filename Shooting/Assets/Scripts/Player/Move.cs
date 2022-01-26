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
        //TurningLock();


        //회전하는 코드
        Turing();
    }

    Vector3 toRotation;
    float z;

    //마우스 위치에 따라 상승한 가속도에 따라 회전
    void Turing()
    {
        /*toRotation.y += 5 * xSpeed * Time.deltaTime;
        toRotation.x += 5 * ySpeed * Time.deltaTime;*/


        //기체 회전 상태에 따른 조작키 연속성 테스트
        //(360도 이상회전으로 인해 위아래가 뒤집어질 경우)
        /* if (rotationState == ZRotationState.Up || rotationState == ZRotationState.UpLeft || rotationState == ZRotationState.UpRight)
         {
             toRotation.y += 5 * xSpeed * Time.deltaTime;
             toRotation.x += 5 * ySpeed * Time.deltaTime;
         }
         else if (rotationState == ZRotationState.Down || rotationState == ZRotationState.DownLeft || rotationState == ZRotationState.DownRight)
         {
             toRotation.y -= 5 * xSpeed * Time.deltaTime;
             toRotation.x += 5 * ySpeed * Time.deltaTime;
         }
         else if(rotationState == ZRotationState.Right)
         {
             toRotation.y += 5 * ySpeed * Time.deltaTime;
             toRotation.x += 5 * xSpeed * Time.deltaTime;
         }*/

        toRotation.y += 5 * xSpeed * Time.deltaTime;
        toRotation.x += 5 * ySpeed * Time.deltaTime;



        //360도를 넘어가면 난리침
        //transform.eulerAngles = new Vector3(-toRotation.x, toRotation.y, transform.eulerAngles.z);

        //정상적으로 회전함
        //왜???
        transform.rotation = Quaternion.Euler(-toRotation.x, toRotation.y, z);

        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(toRotation.x, toRotation.y, z), 0.5f);
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
        if (Input.GetKey(KeyCode.W))
        {
            moveSpeed += Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
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
        transform.position += transform.forward * moveSpeed * 10 * Time.deltaTime;
    }

    //마우스 위치값 받아오는 메서드
    void MousePosition()
    {
        //마우스 위치값에 게임시작시에 받아온 마우스 위치값을 빼서
        //게임시작시 mh, mv가 0으로 시작할수 있게 만듬
        mh = Input.mousePosition.x - mOriPosX;
        mv = Input.mousePosition.y - mOriPosY;

        mousPosLock();

/*        Debug.Log(goHorizontal);
        Debug.Log(goVertical);
*/    }
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

    //a,d 키입력에 따른 스위치문
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
            z -= 100 * Time.deltaTime;
        }

        if(zRotation == ZRotation.toLeftRotation)
        {
            z += 100 * Time.deltaTime;
        }
    }


    //기체의 z축 각도 스위치문
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
    //기체의 z축 각도에 따른 스위치문 상태 변경
    void degree()
    {
        float angle = transform.eulerAngles.z;

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


    //goVertical, goHorizontal의 상태와 ZRot의 기체각도에 따라 회전과 이동속도 제한 메서드
    //이동하는 방향과 기체의 회전 방향이 맞지않을 경우 최대속도를 작게 만듬
    void SpeedClamp()
    {
        //위로 회전
        if (goHorizontal == GoHorizontal.noDir)
        {
            //가로 방향 회전이 없을경우
            if (goVertical == GoVertical.goUp)
            {
                //기체 z축 회전이 없을 경우
                if (rotationState == ZRotationState.Up)
                {
                    ySpeed = Mathf.Clamp(ySpeed, -maxTurningSpeedHigh, maxTurningSpeedHigh);
                    xSpeed = Mathf.Clamp(xSpeed, -maxTurningSpeedLow, maxTurningSpeedLow);
                }
                else if(rotationState == ZRotationState.Down)
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
        else if(goHorizontal == GoHorizontal.goRight)
        {
            if(goVertical == GoVertical.noDir)
            {
                if(rotationState == ZRotationState.UpRight)
                {
                    ySpeed = Mathf.Clamp(ySpeed, -maxTurningSpeedLow, maxTurningSpeedLow);
                    xSpeed = Mathf.Clamp(xSpeed, -maxTurningSpeedHigh, maxTurningSpeedHigh);
                }
                else if(rotationState == ZRotationState.Right)
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
