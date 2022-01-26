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

    //���� ��ȯ �ӵ� �ƽ���
    float maxTurningSpeedLow = 5.0f;
    float maxTurningSpeedHigh = 10.0f;

    //������ȯ�� �Է¹ޱ� ������ ���콺 ��ġ��
    float goDirValue = 100.0f;

    void Start()
    {
        //���� ���۽� ���콺 Ŀ�� �Ⱥ��̵���
        //Cursor.lockState = CursorLockMode.Locked;


        goHorizontal = GoHorizontal.noDir;
        goVertical = GoVertical.noDir;
        zRotation = ZRotation.noDir;
        rotationState = ZRotationState.Up;


        MousePositionInit();
    }

    void MousePositionInit()
    {
        //���� ���۽� ���콺 ��ġ���� �޾ƿ�
        mOriPosX = Input.mousePosition.x;
        mOriPosY = Input.mousePosition.y;
    }

    private void FixedUpdate()
    {
        //ȸ���ִ� ����
        //TurningLock();


        //ȸ���ϴ� �ڵ�
        Turing();
    }

    Vector3 toRotation;
    float z;

    //���콺 ��ġ�� ���� ����� ���ӵ��� ���� ȸ��
    void Turing()
    {
        /*toRotation.y += 5 * xSpeed * Time.deltaTime;
        toRotation.x += 5 * ySpeed * Time.deltaTime;*/


        //��ü ȸ�� ���¿� ���� ����Ű ���Ӽ� �׽�Ʈ
        //(360�� �̻�ȸ������ ���� ���Ʒ��� �������� ���)
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



        //360���� �Ѿ�� ����ħ
        //transform.eulerAngles = new Vector3(-toRotation.x, toRotation.y, transform.eulerAngles.z);

        //���������� ȸ����
        //��???
        transform.rotation = Quaternion.Euler(-toRotation.x, toRotation.y, z);

        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(toRotation.x, toRotation.y, z), 0.5f);
    }
    //x,y�� ȸ�� ���� ����
    void TurningLock()
    {
        //ȸ�� ���� ����
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
        //���콺 ��ġ �޾ƿ��� �޼���
        MousePosition();

        //���콺 ��ġ�� ���� ����������� ȸ������ �����ϴ� �޼���
        MouseHorizontalPosCheck();
        MouseVerticalPosCheck();

        //������ ȸ�����⿡ ���� ȸ�� �۵�
        HorizontalRotation();
        VeticalRotation();

        //��ü z�� ȸ�� ���¿� ���� ���� ����
        ZRot();
        //z�� ���⿡ ���� ZRot����ġ�� ���� ����
        degree();

        //��ü Z�� ȸ�� ����ġ��
        ZAxisRotation();
        //����Ű a,d �Է¿� ����
        //����ġ�� ���º���
        GetKeyZAxis();

        //��,�� ���� ����ġ��
        GoMove();
        //���ӵǴ� ���⿡ ���� GoMove����ġ�� ���� ����
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

    //w�Ǵ� s�� ������ �ִµ��ȸ� �ش� �������� ����
    //������ �ִ� ����(�� �Ǵ� ��)�� ���� move���� ����ġ
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
    //�̵� �ڵ�
    void Go()
    {
        transform.position += transform.forward * moveSpeed * 10 * Time.deltaTime;
    }

    //���콺 ��ġ�� �޾ƿ��� �޼���
    void MousePosition()
    {
        //���콺 ��ġ���� ���ӽ��۽ÿ� �޾ƿ� ���콺 ��ġ���� ����
        //���ӽ��۽� mh, mv�� 0���� �����Ҽ� �ְ� ����
        mh = Input.mousePosition.x - mOriPosX;
        mv = Input.mousePosition.y - mOriPosY;

        mousPosLock();

/*        Debug.Log(goHorizontal);
        Debug.Log(goVertical);
*/    }
    //���콺 �Է°� ���� �޼���
    void mousPosLock()
    {
        //���콺 �Է°��� 300, -300���� ��Ŀ���� ����

        mh = Mathf.Clamp(mh, -300.0f, 300.0f);
        mv = Mathf.Clamp(mv, -300.0f, 300.0f);
    }


    //���콺 ��ġ�� ���� ���� ���� ����ġ
    void MouseHorizontalPosCheck()
    {
        if(mh > -goDirValue && mh < goDirValue)
        {
            goHorizontal = GoHorizontal.noDir;
        }
        else if (mh > goDirValue)
        {
            //mh�� goDirValu(100)���� Ŀ���� ���������� ȸ��
            goHorizontal = GoHorizontal.goRight;
        }
        else if (mh < -goDirValue)
        {
            //mh�� -goDirValu(100)���� �۾����� �������� ȸ��
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


    //���� ȸ�� ����ġ ���� ���� ���¿� ���� ȸ�� �޼���
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
        //ȸ�� ���⿡ �°� ȸ���� ���ӵ� ��
        ySpeed += 2 * Time.deltaTime;
    }
    void GoDown()
    {
        //ȸ�� ���⿡ �°� ȸ���� ���ӵ� ��
        ySpeed -= 2 * Time.deltaTime;
    }
    void StopVeticalRotation()
    {
        //���콺�� �߾����� ���� ���� ȸ�� ������ 0���� �����
        //�� �̻� ȸ������ �ʰ� ����
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
        //ȸ�� ���⿡ �°� ���ӵ� ��
        xSpeed += 2 * Time.deltaTime;

    }
    void GoLeft()
    {
        //ȸ�����⿡ �°� ���ӵ� ��
        xSpeed -= 2 * Time.deltaTime;
    }
    void StopHorizontalRotation()
    {
        //���콺�� ȭ�� �߾����� ���ԵǸ� ȸ�������� 0���� �����
        //�� �̻� ȸ������ �ʰ� ����
        xSpeed = Mathf.Lerp(xSpeed, 0.0f, 1.0f);

        //zSpeed = Mathf.Lerp(zSpeed, 0.0f, 1.0f);
    }

    //a,d Ű�Է¿� ���� ����ġ��
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
    //a,dŰ�� ���� �� �޼����� ����ġ ���� ���¸� �����Ű�� �ڵ�
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
    //����ġ ���� ���¿� ���� ��ü�� z������ ȸ��
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


    //��ü�� z�� ���� ����ġ��
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
    //��ü�� z�� ������ ���� ����ġ�� ���� ����
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
    //���� �Ǵ� �����϶�
    //degree�� ���°� up�Ǵ� down


    //goVertical, goHorizontal�� ���¿� ZRot�� ��ü������ ���� ȸ���� �̵��ӵ� ���� �޼���
    //�̵��ϴ� ����� ��ü�� ȸ�� ������ �������� ��� �ִ�ӵ��� �۰� ����
    void SpeedClamp()
    {
        //���� ȸ��
        if (goHorizontal == GoHorizontal.noDir)
        {
            //���� ���� ȸ���� �������
            if (goVertical == GoVertical.goUp)
            {
                //��ü z�� ȸ���� ���� ���
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
