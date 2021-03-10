using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementEnable : MonoBehaviour
{
    public Text GetInCar;

    private void Start()
    {
        GetInCar.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GetInCar.enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        GetInCar.enabled = false;
    }
}
