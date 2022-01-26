using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUseMouse : MonoBehaviour
{

    Vector2 screenCenter,lookInput,mouseDistance;

    Move move;

    private void Start()
    {
        screenCenter.x = Screen.width * 0.5f;
        screenCenter.y = Screen.height * 0.5f;

        move = transform.GetComponent<Move>();
    }

    private void Update()
    {
        lookInput.x = Input.mousePosition.x;
        lookInput.y = Input.mousePosition.y;

        mouseDistance.x = (lookInput.x - screenCenter.x) / screenCenter.x;
        mouseDistance.y = (lookInput.y - screenCenter.y) / screenCenter.y;

        transform.Rotate(-mouseDistance.y * 90.0f * Time.deltaTime, mouseDistance.x * 90.0f * Time.deltaTime, transform.rotation.z);
        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(-mouseDistance.y, mouseDistance.x, transform.eulerAngles.z)), 10.0f * Time.deltaTime);
    }

}
