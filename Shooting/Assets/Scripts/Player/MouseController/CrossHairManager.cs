using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossHairManager : MonoBehaviour
{
    public GameObject dirMousePointer;

    private RectTransform canvasRect;

    public Canvas canvas;

    Vector3 canvasCenter;

    private GameObject Arrow;
    private GameObject CenterImage;

    public float mouseCenterPos = 10.0f;

    Vector2 screenCenter;

    float mh;
    float mv;

    float mOriPosX;
    float mOriPosY;


    void Start()
    {
        canvasRect = canvas.GetComponent<RectTransform>();

        Arrow = dirMousePointer.transform.GetChild(0).gameObject;
        CenterImage = dirMousePointer.transform.GetChild(1).gameObject;



        mh = Input.mousePosition.x - mOriPosX;
        mv = Input.mousePosition.y - mOriPosY;
    }

    // Update is called once per frame
    void Update()
    {
        //캔버스 중앙 구하기
        canvasCenter = new Vector3(canvasRect.rect.width / 2f, canvasRect.rect.height / 2f, 0f) * canvasRect.localScale.x;


        //마우스 위치가 캔버스 중앙일때 
        if(Vector3.Distance(canvasCenter,dirMousePointer.transform.position) < mouseCenterPos)
        {
            CenterImage.SetActive(true);
            Arrow.SetActive(false);

            //마우스 위치가 캔버스중앙이 아닐때 회전했던 값에 맞춰 centerimage도 회전해서 회전값 초기화
            dirMousePointer.transform.rotation = Quaternion.Euler(0, 0, 0);
            //마우스가 미세하게 움직여도 바로 회전하지않기때문에 마우스위치가 캔버스 중앙일때는 centerimage가 마우스를 따라 움직이지 않도록 고정
            CenterImage.transform.position = canvasCenter;
        }
        //아닐때
        else if(Vector3.Distance(canvasCenter, dirMousePointer.transform.position) >= mouseCenterPos)
        {
            CenterImage.SetActive(false);
            Arrow.SetActive(true);

            float angle = Vector3.SignedAngle(Vector3.right, canvasCenter - dirMousePointer.transform.position, Vector3.forward);
            //                                                                                                  시계방향 회전

            dirMousePointer.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
