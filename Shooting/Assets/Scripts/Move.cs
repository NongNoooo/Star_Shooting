using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool goDown = false;
    public bool goUp = false;
    public bool goLeft = false;
    public bool goRight = false;


    float mh;
    float mv;

    float xSpeed;
    float ySpeed;

    // Update is called once per frame
    void Update()
    {
        //���콺 X��Y�� �Է¹���
        /*        mh = Input.GetAxis("Mouse X");
                mv = Input.GetAxis("Mouse Y");
        */

        mh = Input.mousePosition.x;
        mv = Input.mousePosition.y;

        LeftRight();
        UpDown();

        Hvelocity();


        Vector3 dir = new Vector3(0, xSpeed, 0.0f);



        transform.eulerAngles += 10 * dir * Time.deltaTime;
    }

    void UpDown()
    {
        //�Է¹��⿡ ���� �׿��´� �Ұ��� TRUE�� ����
        if (mv < 0)
        {
            goUp = false;
            goDown = true;
        }
        else if (mh > 0)
        {
            goDown = false;
            goUp = true;
        }
    }

    void LeftRight()
    {
        if (mh < 0)
        {
            goRight = false;
            goLeft = true;
        }
        else if (mv > 0)
        {
            goLeft = false;
            goRight = true;
        }
    }

    void Hvelocity()
    {
        //���� ���� �Ұ��� ���� �� �������� �ӵ� ���� ����
        if (goRight)
        {
            xSpeed += 1 * Time.deltaTime;
        }
        if (goLeft)
        {
            xSpeed -= 1 * Time.deltaTime;
        }

     /*   if (xSpeed < 0)
        {
            if (xSpeed > -10)
            {
                xSpeed = 10;
            }
        }
        else if(xSpeed > 0)
        {
            if()
        }*/
    }
}
