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

    private void Update()
    {
        if(target == null)
        {
            Destroy(this.gameObject);
        }
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


    void SetIndicatorPosition()
    {
        if(target == null)
        {
            return;
        }
        //스크린스페이스 기준 대상 위치 가져옴
        Vector3 indicatorPosition = mainCamera.WorldToScreenPoint(target.transform.position);

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
            indicatorPosition = OutOfRangeindicatorPositionBack(indicatorPosition);
            targetOutOfSight(true, indicatorPosition);
        }
        else
        {
            //표적이 카메라 뒤에 잇을 경우 마크 위치 반전
            indicatorPosition *= -1f;

            //마크 위치 설정
            indicatorPosition = OutOfRangeindicatorPositionBack(indicatorPosition);
            targetOutOfSight(true, indicatorPosition);

        }

        //마크 위치 설정
        rectTransform.position = indicatorPosition;

    }

    //대상이 범위 밖으로 나갔을때
    //대상이 카메라를 기준으로 뒤에 있을때
    private Vector3 OutOfRangeindicatorPositionBack(Vector3 indicatorPosition)
    {
        indicatorPosition.z = 0f;

        //캔버스 중심 계산       
        Vector3 canvasCenter = new Vector3(canvasRect.rect.width / 2f, canvasRect.rect.height / 2f, 0f) * canvasRect.localScale.x;
        indicatorPosition -= canvasCenter;

        //현재 인디케이터 위치가 x,y축 확인
        float divX = (canvasRect.rect.width / 2f - outOfSightOffest) / Mathf.Abs(indicatorPosition.x);
        float divY = (canvasRect.rect.height / 2f - outOfSightOffest) / Mathf.Abs(indicatorPosition.y);

        //인디케이터의 x,y축 비교
        if (divX < divY)
        {
            //두 백터 사이 각 비교
            //                                 회전 축             비교할 백	        기준 백터
            float angle = Vector3.SignedAngle(Vector3.right, indicatorPosition, Vector3.forward);

            //0또는 양수면 1반환 음수면 -1반환
            indicatorPosition.x = Mathf.Sign(indicatorPosition.x) * (canvasRect.rect.width * 0.5f - outOfSightOffest) * canvasRect.localScale.x;
            // 탄젠트 세타
            indicatorPosition.y = Mathf.Tan(Mathf.Deg2Rad * angle) * indicatorPosition.x;
        }
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


    //대상이 화면 밖으로 나갔을때
    private void targetOutOfSight(bool oos, Vector3 indicatorPosition)
    {
        if (oos)
        {
            //사각형 비활성화 삼각형 이미지 활성화
            if (OffScreenTargetIndicator.gameObject.activeSelf == false) OffScreenTargetIndicator.gameObject.SetActive(true);
            if (TargetIndicatorImage.isActiveAndEnabled == true) TargetIndicatorImage.enabled = false;

            //삼각형 이미지를 중심을 기준으로 타겟이 있는 위치로 회전
            OffScreenTargetIndicator.rectTransform.rotation = Quaternion.Euler(rotationOutOfSightTargetindicator(indicatorPosition));
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
        //캔버스 중심 계산
        Vector3 canvasCenter = new Vector3(canvasRect.rect.width / 2f, canvasRect.rect.height / 2f, 0f) * canvasRect.localScale.x;

        //인디케이터 위치에서 캔버스 각도 계산
        float angle = Vector3.SignedAngle(Vector3.up, indicatorPosition - canvasCenter, Vector3.forward);

        //계산한 각도 값 리턴
        return new Vector3(0f, 0f, angle);
    }
}
