using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
        //???? ?????? ?????? ???? ??????????
        //Cursor.lockState = CursorLockMode.Locked;


        goHorizontal = GoHorizontal.noDir;
        goVertical = GoVertical.noDir;
        zRotation = ZRotation.noDir;
        rotationState = ZRotationState.Up;


        MousePositionInit();
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
        //???????? ????
        //TurningLock();


        //???????? ????
        Turing();
    }

    Vector3 toRotation;
    public float z;
    Vector3 dir;
    public Transform player;


    //?????? ?????? ???? ?????? ???????? ???? ????
    void Turing()
    {
        //????????
        toRotation.y += 5 * xSpeed * Time.deltaTime;
        toRotation.x += 5 * ySpeed * Time.deltaTime;



        //?? ??
        lookInput.x = Input.mousePosition.x;
        lookInput.y = Input.mousePosition.y;

        mouseDistance.x = (lookInput.x - screenCenter.x) / screenCenter.x;
        mouseDistance.y = (lookInput.y - screenCenter.y) / screenCenter.y;

        //transform.Rotate(-mouseDistance.y * 90.0f * Time.deltaTime, mouseDistance.x * 90.0f * Time.deltaTime, transform.rotation.z);
        if (xSpeed > 0 || xSpeed < 0 || ySpeed > 0 || ySpeed < 0)
        {
            transform.Rotate(-mouseDistance.y * 90.0f * Time.deltaTime, mouseDistance.x * 90.0f * Time.deltaTime, 0);

        }

        //lead.transform.position = new Vector3(-mouseDistance.y + front.transform.position.x, mouseDistance.x + front.transform.position.y, front.transform.position.z);
        //lead.transform.position = new Vector3(-mouseDistance.y+transform.position.x ,mouseDistance.x+transform.position.x, front.transform.position.z);
        //lead.transform.position = new Vector3(mh + front.transform.position.x, mv + front.transform.position.y, front.transform.position.z);
        lead.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
        //lead.transform.RotateAround(player.transform.position, Vector3.zero,1.0f*Time.deltaTime);
        //transform.RotateAround(player.transform.position, Vector3.up, 1.0f * Time.deltaTime);

        //360???? ???????? ??????
        //transform.eulerAngles = new Vector3(-toRotation.x, toRotation.y, transform.eulerAngles.z);

        //?????????? ??????
        //?????
        //transform.rotation = Quaternion.Euler(-toRotation.x, toRotation.y, z);


        //dir = new Vector3(-toRotation.x, toRotation.y, transform.eulerAngles.z);
        //dir = new Vector3(-mv, mh, transform.eulerAngles.z);
    }

    public GameObject zRotObj;
    public GameObject camPos;
    public GameObject lead;
    public GameObject front;


    //x,y?? ???? ???? ????
    void TurningLock()
    {
        //???? ???? ????
        //toRotation.x = Mathf.Clamp(toRotation.x, -120.0f, 120.0f);
        if (toRotation.x < -120.0f)
        {
            toRotation.x = -120.0f;
        }

        toRotation.y = Mathf.Clamp(toRotation.y, -120.0f, 120.0f);
        //toRotation.z = Mathf.Clamp(toRotation.z, -30.0f, 30.0f);
    }

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

        //???? z?? ???? ?????? ???? ???? ????
        ZRot();
        //z?? ?????? ???? ZRot???????? ???? ????
        degree();

        //???? Z?? ???? ????????
        ZAxisRotation();
        //?????? a,d ?????? ????
        //???????? ????????
        GetKeyZAxis();

        //??,?? ???? ????????
        GoMove();
        //???????? ?????? ???? GoMove???????? ???? ????
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

    //w???? s?? ?????? ?????????? ???? ???????? ????
    //?????? ???? ????(?? ???? ??)?? ???? move???? ??????
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
        else if (moveSpeed < 0)
        {
            move = Moving.goBack;
        }
    }
    //???? ????
    void Go()
    {
        transform.position += transform.forward * moveSpeed * 10 * Time.deltaTime;
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

    //a,d ???????? ???? ????????
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
    //a,d???? ???? ?? ???????? ?????? ???? ?????? ?????????? ????
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
    //?????? ???? ?????? ???? ?????? z?????? ????
    void GoZAxisRotation()
    {
        Vector3 tr = transform.eulerAngles;

        if (zRotation == ZRotation.toRightRotation)
        {
            //z -= 100 * Time.deltaTime;
            transform.Rotate(0, 0, -100 * Time.deltaTime);
        }
        else if (zRotation == ZRotation.noDir)
        {
            z = 0;
        }

        if (zRotation == ZRotation.toLeftRotation)
        {
            transform.Rotate(0, 0, 100 * Time.deltaTime);

            //z += 100 * Time.deltaTime;
        }
        else if (zRotation == ZRotation.noDir)
        {
            z = 0;
        }
    }


    //?????? z?? ???? ????????
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
    //?????? z?? ?????? ???? ???????? ???? ????
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
    //???? ???? ????????
    //degree?? ?????? up???? down


    //goVertical, goHorizontal?? ?????? ZRot?? ?????????? ???? ?????? ???????? ???? ??????
    //???????? ?????? ?????? ???? ?????? ???????? ???? ?????????? ???? ????
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
