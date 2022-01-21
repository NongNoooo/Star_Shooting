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
        //ȸ���ϴ� �ڵ�
        Turing();
    }

    Vector2 toRotation;

    //���콺 ��ġ�� ���� ����� ���ӵ��� ���� ȸ��
    void Turing()
    {
        Vector2 dir = new Vector2(ySpeed, xSpeed);

        //90������ �� �̻� ȸ������
        //���Ϸ� �ޱ��� ���������� �������� ����
        //���� �Ʒ�ó�� ���Թ������ �ۼ��ؾ���
        //transform.eulerAngles += dir * 10/*�����ǰ�*/ * Time.deltaTime;

        //����3 ������ ������ �Ŀ�
        //toRotation += 10 * dir * Time.deltaTime;
        toRotation.x += 10 * ySpeed * Time.deltaTime;
        toRotation.y += 10 * xSpeed * Time.deltaTime;

        TurningLock();

        //������ x�� �����ϸ� ���������� ȸ����
        transform.eulerAngles = new Vector3(toRotation.x, toRotation.y, transform.eulerAngles.z);
        



        //���������� ȸ����
        //�׷��� �ʹ� �ﰢ������ ȸ����
        /* transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(dir), 1.0f);

         if (transform.rotation.eulerAngles.x < -120.0f)
         {
             transform.eulerAngles = new Vector3(-120.0f, transform.rotation.y, transform.rotation.z);
         }*/
    }
    //x,y�� ȸ�� ���� ����
    void TurningLock()
    {
        //ȸ�� ���� ����
        toRotation.x = Mathf.Clamp(toRotation.x, -120.0f, 120.0f);
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

        TrueSpeedDir();

        //��ü Z�� ȸ�� ����ġ��
        ZAxisRotation();
        //����Ű a,d �Է¿� ����
        //����ġ�� ���º���
        GetKeyZAxis();

    }

    //���콺 ��ġ�� �޾ƿ��� �޼���
    void MousePosition()
    {
        //���콺 ��ġ���� ���ӽ��۽ÿ� �޾ƿ� ���콺 ��ġ���� ����
        //���ӽ��۽� mh, mv�� 0���� �����Ҽ� �ְ� ����
        mh = Input.mousePosition.x - mOriPosX;
        mv = Input.mousePosition.y - mOriPosY;

        mousPosLock();

        Debug.Log(goHorizontal);
        Debug.Log(goVertical);
    }
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
        else if (mv > goDirValue)
        {
            goVertical = GoVertical.goDown;
        }
        else if (mv < -goDirValue)
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
        ySpeed += 5 * Time.deltaTime;
    }
    void GoDown()
    {
        //ȸ�� ���⿡ �°� ȸ���� ���ӵ� ��
        ySpeed -= 5 * Time.deltaTime;
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
        xSpeed += 5 * Time.deltaTime;

    }
    void GoLeft()
    {
        //ȸ�����⿡ �°� ���ӵ� ��
        xSpeed -= 5 * Time.deltaTime;
    }
    void StopHorizontalRotation()
    {
        //���콺�� ȭ�� �߾����� ���ԵǸ� ȸ�������� 0���� �����
        //�� �̻� ȸ������ �ʰ� ����
        xSpeed = Mathf.Lerp(xSpeed, 0.0f, 1.0f);
        //zSpeed = Mathf.Lerp(zSpeed, 0.0f, 1.0f);
    }

    //z�� ����ġ��
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

    //ȸ�� ���ӵ� �ִ�ӵ� ����
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
