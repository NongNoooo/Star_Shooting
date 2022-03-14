using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetObject : MonoBehaviour
{
    private void Awake()
    {
        UIController ui = GetComponentInParent<UIController>();
        if(ui == null)
        {
            ui = GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>();
        }

        ui.AddTargetIndicator(this.gameObject);
    }
}
