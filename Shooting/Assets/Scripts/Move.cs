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

    public GoHorizontal goHorizontal;
    public GoVertical goVertical;

    float mh;
    float mv;

    public float xSpeed;
    public float ySpeed;

    float mOriPosX;
    float mOriPosY;

    //���� ��ȯ �ӵ� �ƽ���
    float maxTurningSpeed = 10.0f;

    //������ȯ�� �Է¹ޱ� ������ ���콺 ��ġ��
    float goDirValue = 100.0f;

    void Start()
    {
        //���� ���۽� ���콺 Ŀ�� �Ⱥ��̵���
        Cursor.lockState = CursorLockMode.Locked;
        goHorizontal = GoHorizontal.noDir;
        goVertical = GoVertical.noDir;

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

    Vector3 toRotation;

    void Turing()
    {
        Vector3 dir = new Vector3(-ySpeed, xSpeed, 0.0f);

        //90������ �� �̻� ȸ������
        //���Ϸ� �ޱ��� ���������� �������� ����
        //���� �Ʒ�ó�� ���Թ������ �ۼ��ؾ���
        //transform.eulerAngles += dir * 10/*�����ǰ�*/ * Time.deltaTime;

        //����3 ������ ������ �Ŀ�
        toRotation += 10 * dir * Time.deltaTime;

        TurningLock();

        //������ x�� �����ϸ� ���������� ȸ����
        transform.eulerAngles = toRotation;



        //���������� ȸ����
        //�׷��� �ʹ� �ﰢ������ ȸ����
        /* transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(dir), 1.0f);

         if (transform.rotation.eulerAngles.x < -120.0f)
         {
             transform.eulerAngles = new Vector3(-120.0f, transform.rotation.y, transform.rotation.z);
         }*/
    }

    void TurningLock()
    {
        //ȸ�� ���� ����
        toRotation.x = Mathf.Clamp(toRotation.x, -120.0f, 120.0f);
        toRotation.y = Mathf.Clamp(toRotation.y, -120.0f, 120.0f);
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
    }

    void MousePosition()
    {
        //���콺 ��ġ���� ���ӽ��۽ÿ� �޾ƿ� ���콺 ��ġ���� ����
        //���ӽ��۽� mh, mv�� 0���� �����Ҽ� �ְ� ����
        mh = Input.mousePosition.x - mOriPosX;
        mv = Input.mousePosition.y - mOriPosY;

        mousPosLock();

        Debug.Log("x = " + mh + "  y = " + mv);
    }
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

        SpeedLimit(ref ySpeed);
    }
    void GoDown()
    {
        //ȸ�� ���⿡ �°� ȸ���� ���ӵ� ��
        ySpeed -= 5 * Time.deltaTime;

        SpeedLimit(ref ySpeed);
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


        SpeedLimit(ref xSpeed);
    }
    void GoLeft()
    {
        //ȸ�����⿡ �°� ���ӵ� ��
        xSpeed -= 5 * Time.deltaTime;

        SpeedLimit(ref xSpeed);
    }
    void StopHorizontalRotation()
    {
        //���콺�� ȭ�� �߾����� ���ԵǸ� ȸ�������� 0���� �����
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
        xSpeed = Mathf.Lerp(xSpeed, 0.0f, 1.0f);
    }


    //ȸ�� ���ӵ� �ִ�ӵ� ����
    void SpeedLimit(ref float speed)
    {
        if(speed > maxTurningSpeed)
        {
            speed = maxTurningSpeed;
        }
        else if(speed < -maxTurningSpeed)
        {
            speed = -maxTurningSpeed;
        }
    }

}
