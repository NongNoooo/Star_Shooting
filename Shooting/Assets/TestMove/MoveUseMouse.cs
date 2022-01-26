using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUseMouse : MonoBehaviour
{
    public GameObject lead;
    public GameObject front;

    float mh;
    float mv;

    float mOriPosX;
    float mOriPosY;

    // Start is called before the first frame update
    void Start()
    {
        mOriPosX = Input.mousePosition.x;
        mOriPosY = Input.mousePosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        MouseOriginPos();
        LeadCursorMove();
    }

    void MouseOriginPos()
    {
        mv = Input.mousePosition.x - mOriPosX;
        mh = Input.mousePosition.y - mOriPosY;

    }

    void LeadCursorMove()
    {
        lead.transform.position = new Vector3(mv, mh, front.transform.position.z);
    }
}
