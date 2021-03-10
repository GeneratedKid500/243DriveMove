using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetOutOfCar : MonoBehaviour
{
    public Text GetOuttaCar;

    GameObject player;
    GameObject carCam;

    CarController carController;

    private void Start()
    {
        GetOuttaCar.enabled = false;
        this.enabled = false;
    }

    void Update()
    {
        if (!GetOuttaCar.isActiveAndEnabled)
        {
            GetOuttaCar.enabled = true;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            carCam.SetActive(false);
            carController.isDriven = false;
            player.SetActive(true);
            player.transform.parent = null;
            this.enabled = false;
            GetOuttaCar.enabled = false;
        }
    }

    public void SetVars(GameObject tPlayer, GameObject tCarCam, CarController tCarController)
    {
        player = tPlayer;
        carCam = tCarCam;
        carController = tCarController;
    }
}
