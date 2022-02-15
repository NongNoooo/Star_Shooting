using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    GameObject[] enemy;

    public float enemyCount;

    TextMeshProUGUI txt;

    public GameObject enemyCountTxt;

    void Awake()
    {
        enemy = GameObject.FindGameObjectsWithTag("Enemy");

        enemyCount = enemy.Length;

        txt = enemyCountTxt.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        txt.text = "Enemy : " + enemyCount;
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

        Debug.Log("GameEnd");
    }
}
