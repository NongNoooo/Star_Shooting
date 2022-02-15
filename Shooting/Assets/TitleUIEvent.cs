using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUIEvent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public GameObject unFilled;
    public GameObject filled;

    public void MenuObjToggle()
    {
        //마우스가 메뉴UI에 올라 갔을 경우 현재 상태의 반대 상태로 변경
        // 꺼져있으면 -> 켜짐
        // 켜져있으면 -> 꺼짐

        bool isFilled = filled.activeSelf;
        bool isUnFilled = unFilled.activeSelf;

        unFilled.SetActive(!isUnFilled);
        filled.SetActive(!isFilled);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(2);
    }
}
