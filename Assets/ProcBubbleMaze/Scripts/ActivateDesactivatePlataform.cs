using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateDesactivatePlataform : MonoBehaviour
{
    public GameObject target;
    public bool activate = false;
    public bool desactivate = false;
    public RuntimePlatform platform;

    void Start()
    {
        if(Application.platform == platform) {
            if (activate) target.SetActive(true);
            if (desactivate) target.SetActive(false);
        }
    }
}
