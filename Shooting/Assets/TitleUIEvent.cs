using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUIEvent : MonoBehaviour
{

    public GameObject unFilled;
    public GameObject filled;

    public void MenuObjToggle()
    {
        bool isFilled = filled.activeSelf;
        bool isUnFilled = unFilled.activeSelf;

        //현재 상태의 반대로 - 토글
        unFilled.SetActive(!isUnFilled);
        filled.SetActive(!isFilled);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(2);
    }

    public void GoMenu()
    {
        SceneManager.LoadScene(1);
    }

    public void EndGame()
    {
        Application.Quit();
    }
}
