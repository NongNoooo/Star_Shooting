using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    GameObject player;
    PlayerController pc;

    GameObject[] enemy;
    GameObject[] target;

    public float enemyCount;

    TextMeshProUGUI enemyTxt;
    public GameObject enemyCountTxt;

    TextMeshProUGUI playerHpTxt;
    public GameObject playerHpTxtObj;
    float hp;

    public GameObject endGame;


    void Awake()
    {
        enemy = GameObject.FindGameObjectsWithTag("Enemy");
        target = GameObject.FindGameObjectsWithTag("Target");

        enemyCount = enemy.Length;

        enemyTxt = enemyCountTxt.GetComponent<TextMeshProUGUI>();

        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();
        playerHpTxt = playerHpTxtObj.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        //hp반올림
        hp = Mathf.Round(pc.hp);

        //tostring으로 문자로 반환
        playerHpTxt.text = hp.ToString();

        enemyTxt.text = "Enemy : " + enemyCount;
    }

    public void EnemyDie()
    {
        enemyCount--;
        GameEnd();
    }


    void GameEnd()
    {
        if(enemyCount > 0)
        {
            return;
        }

        endGame.SetActive(true);
        player.SetActive(false);
        Debug.Log("GameEnd");
    }
}
