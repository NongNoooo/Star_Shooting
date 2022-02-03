using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum DogFightState
    {
        Chasing,
        Chased,
        DoNothing,
    }

    public DogFightState dogFightState;

    GameObject player;

    public GameObject[] Rail;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        dogFightState = DogFightState.DoNothing;

        Rail = GameObject.FindGameObjectsWithTag("Rail");
    }

    void Update()
    {
        DogFight();
        LookPlayer();
    }

    void LookPlayer()
    {
        Vector3 dir = player.transform.position - transform.position;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 0.5f * Time.deltaTime);
    }

    void DogFight()
    {
        switch (dogFightState)
        {
            case DogFightState.Chasing:
                break;
            case DogFightState.Chased:
                break;
            case DogFightState.DoNothing:
                RailRider();
                break;
        }
    }

    public float[] dis;

    void RailRider()
    {
        dis = new float[Rail.Length];
         for(int i = 0; i < Rail.Length; i++)
        {
            dis[i] = (Rail[i].transform.position - transform.position).magnitude;
        }
    }
}
