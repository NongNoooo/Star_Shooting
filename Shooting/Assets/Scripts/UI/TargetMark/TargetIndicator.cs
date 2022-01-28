using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetIndicator : MonoBehaviour
{
    public Image TargetIndicatorImage;
    public Image OffScreenTargetIndicator;
    public float OutOfSightOffset = 20f;
    private float outOfSightOffest { get { return OutOfSightOffset /* canvasRect.localScale.x*/; } }

    public GameObject target;
    private Camera mainCamera;
    private RectTransform canvasRect;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }


    public void InitialiseTargetIndicator(GameObject target, Camera mainCamera, Canvas canvas)
    {
        this.target = target;
        this.mainCamera = mainCamera;
        canvasRect = canvas.GetComponent<RectTransform>();
    }

    public void UpdateTargetIndicator()
    {
        SetIndicatorPosition();

        //거리 표시 조정
        //범위밖으로 나가거나 들어올때 끄고 켬
        //타겟이 TargetObject스크립트를 가지고잇고 대상으로 정해질경우 작동
    }


    protected void SetIndicatorPosition()
    {

        //스크린스페이스 기준 대상 위치 가져옴
        Vector3 indicatorPosition = mainCamera.WorldToScreenPoint(target.transform.position);
        //Debug.Log("GO: "+ gameObject.name + "; slPos: " + indicatorPosition + "; cvWidt: " + canvasRect.rect.width + "; cvHeight: " + canvasRect.rect.height);

        //대상이 카메라 범위내에 있을 경우
        if (indicatorPosition.z >= 0f & indicatorPosition.x <= canvasRect.rect.width * canvasRect.localScale.x
         & indicatorPosition.y <= canvasRect.rect.height * canvasRect.localScale.x & indicatorPosition.x >= 0f & indicatorPosition.y >= 0f)
        {

            //카메라와 거리가 너무 멀어져 오류 뱉어서 0으로 고정
            indicatorPosition.z = 0f;

            //대상이 카메라 시야 내부에 있으면 화살표 끔
            targetOutOfSight(false, indicatorPosition);
        }

        //대상이 카메라 범위 밖에 있을 경우
        else if (indicatorPosition.z >= 0f)
        {
            //화살표 표시 보여줌
            indicatorPosition = OutOfRangeindicatorPositionB(indicatorPosition);
            targetOutOfSight(true, indicatorPosition);
        }
        else
        {
            //Invert indicatorPosition! Otherwise the indicator's positioning will invert if the target is on the backside of the camera!
            //표적이 카메라 뒤에 잇을 경우 마크 위치 반전
            indicatorPosition *= -1f;

            //마크 위치 설정
            indicatorPosition = OutOfRangeindicatorPositionB(indicatorPosition);
            targetOutOfSight(true, indicatorPosition);

        }

        //마크 위치 설정
        rectTransform.position = indicatorPosition;

    }

    //대상이 범위 밖으로 나갔을때
    private Vector3 OutOfRangeindicatorPositionB(Vector3 indicatorPosition)
    {
        //카메라 범위 밖으로 나갔을때 마크 문제생기는거 방지?
        indicatorPosition.z = 0f;

        //캔버스 중심 계산
        
        Vector3 canvasCenter = new Vector3(canvasRect.rect.width / 2f, canvasRect.rect.height / 2f, 0f) * canvasRect.localScale.x;
        indicatorPosition -= canvasCenter;

        //대상으로 지정할 벡터가 캔버스 직경의 y 경계와 교차하는지(첫 번째) 또는 벡터가 x 경계와 교차하는지(첫 번째) 계산
        //최대값으로 설정해야 하는 테두리와 표시기를 이동해야 하는 테두리(위/아래 또는 왼쪽/오른쪽)를 확인
        float divX = (canvasRect.rect.width / 2f - outOfSightOffest) / Mathf.Abs(indicatorPosition.x);
        float divY = (canvasRect.rect.height / 2f - outOfSightOffest) / Mathf.Abs(indicatorPosition.y);

        //x 경계와 먼저 교차하는 경우 x-one을 경계에 놓고 y-one을 적절히 조절합니다(Trigonometry)
        if (divX < divY)
        {
            float angle = Vector3.SignedAngle(Vector3.right, indicatorPosition, Vector3.forward);
            indicatorPosition.x = Mathf.Sign(indicatorPosition.x) * (canvasRect.rect.width * 0.5f - outOfSightOffest) * canvasRect.localScale.x;
            indicatorPosition.y = Mathf.Tan(Mathf.Deg2Rad * angle) * indicatorPosition.x;
        }

        //y 테두리와 먼저 교차하는 경우 y-one을 테두리에 놓고 x-one을 적절히 조절합니다(Trigonometry).
        else
        {
            float angle = Vector3.SignedAngle(Vector3.up, indicatorPosition, Vector3.forward);

            indicatorPosition.y = Mathf.Sign(indicatorPosition.y) * (canvasRect.rect.height / 2f - outOfSightOffest) * canvasRect.localScale.y;
            indicatorPosition.x = -Mathf.Tan(Mathf.Deg2Rad * angle) * indicatorPosition.y;
        }

        //인디케이터 포지션을 실제 rectTransform의 위치로 변경
        indicatorPosition += canvasCenter;
        return indicatorPosition;
    }



    private void targetOutOfSight(bool oos, Vector3 indicatorPosition)
    {
        //대상이 카메라 밖으로 나갔을 경우
        if (oos)
        {
            //일부 콘텐츠 활성화 및 비활성화
            if (OffScreenTargetIndicator.gameObject.activeSelf == false) OffScreenTargetIndicator.gameObject.SetActive(true);
            if (TargetIndicatorImage.isActiveAndEnabled == true) TargetIndicatorImage.enabled = false;

            //Set the rotation of the OutOfSight direction indicator
            OffScreenTargetIndicator.rectTransform.rotation = Quaternion.Euler(rotationOutOfSightTargetindicator(indicatorPosition));

            //outOfSightArrow.rectTransform.rotation  = Quaternion.LookRotation(indicatorPosition- new Vector3(canvasRect.rect.width/2f,canvasRect.rect.height/2f,0f)) ;
            /*outOfSightArrow.rectTransform.rotation = Quaternion.LookRotation(indicatorPosition);
            viewVector = indicatorPosition- new Vector3(canvasRect.rect.width/2f,canvasRect.rect.height/2f,0f);
            
            //Debug.Log("CanvasRectCenter: " + canvasRect.rect.center);
            outOfSightArrow.rectTransform.rotation *= Quaternion.Euler(0f,90f,0f);*/
        }

        //In case that the indicator is InSight, turn on the inSight stuff and turn off the OOS stuff.
        else
        {
            if (OffScreenTargetIndicator.gameObject.activeSelf == true) OffScreenTargetIndicator.gameObject.SetActive(false);
            if (TargetIndicatorImage.isActiveAndEnabled == false) TargetIndicatorImage.enabled = true;
        }
    }


    private Vector3 rotationOutOfSightTargetindicator(Vector3 indicatorPosition)
    {
        //Calculate the canvasCenter
        Vector3 canvasCenter = new Vector3(canvasRect.rect.width / 2f, canvasRect.rect.height / 2f, 0f) * canvasRect.localScale.x;

        //Calculate the signedAngle between the position of the indicator and the Direction up.
        float angle = Vector3.SignedAngle(Vector3.up, indicatorPosition - canvasCenter, Vector3.forward);

        //return the angle as a rotation Vector
        return new Vector3(0f, 0f, angle);
    }
}
