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

    public GoHorizontal goHorizontal;
    public GoVertical goVertical;
    public ZRotation zRotation;


    float mh;
    float mv;

    public float xSpeed;
    public float ySpeed;
    public float zSpeed;

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
        //회전하는 코드
        Turing();
    }

    Vector2 toRotation;

    //마우스 위치에 따라 상승한 가속도에 따라 회전
    void Turing()
    {
        Vector2 dir = new Vector2(ySpeed, xSpeed);

        //90도에서 더 이상 회전안함
        //오일러 앵글은 음수값으로 더해지지 못함
        //따라서 아래처럼 대입방식으로 작성해야함
        //transform.eulerAngles += dir * 10/*임의의값*/ * Time.deltaTime;

        //백터3 변수에 대입한 후에
        //toRotation += 10 * dir * Time.deltaTime;
        toRotation.x += 10 * ySpeed * Time.deltaTime;
        toRotation.y += 10 * xSpeed * Time.deltaTime;

        TurningLock();

        //각도를 x로 지정하면 정상적으로 회전함
        transform.eulerAngles = new Vector3(toRotation.x, toRotation.y, transform.eulerAngles.z);
        



        //정상적으로 회전함
        //그런데 너무 즉각적으로 회전함
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
        toRotation.x = Mathf.Clamp(toRotation.x, -120.0f, 120.0f);
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

        TrueSpeedDir();

        //기체 Z축 회전 스위치문
        ZAxisRotation();
        //방향키 a,d 입력에 따라
        //스위치문 상태변경
        GetKeyZAxis();

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
        else if (mv > goDirValue)
        {
            goVertical = GoVertical.goDown;
        }
        else if (mv < -goDirValue)
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
        ySpeed += 5 * Time.deltaTime;
    }
    void GoDown()
    {
        //회전 방향에 맞게 회전에 가속도 줌
        ySpeed -= 5 * Time.deltaTime;
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
        xSpeed += 5 * Time.deltaTime;

    }
    void GoLeft()
    {
        //회전방향에 맞게 가속도 줌
        xSpeed -= 5 * Time.deltaTime;
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


    void TrueSpeedDir()
    {
        float angle = transform.eulerAngles.z;

        Debug.Log(angle);

        if (goHorizontal == GoHorizontal.goLeft)
        {
            if (angle >= 0 && angle < 30)
            {
                Debug.Log("Low");
            }
            else if (angle >= 30 && angle <= 90)
            {
                Debug.Log("High");
            }
            else if(angle > 90 && angle <= 120)
            {
                Debug.Log("Low");
            }
            else if(angle > 120 && angle < 180)
            {
                Debug.Log("Low");
            }
            else if(angle >= 210 && angle <= 270)
            {
                Debug.Log("High");
            }
        }
        else if (goHorizontal == GoHorizontal.goRight)
        {
            if (-angle < -30)
            {
                Debug.Log("Low");
            }
            else if (-angle >= -30 && -angle >= -90)
            {
                Debug.Log("High");
            }
        }
    }
}
