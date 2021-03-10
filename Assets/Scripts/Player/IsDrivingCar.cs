using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsDrivingCar : MonoBehaviour
{
    public GameObject player;

    Transform playerCam;
    GameObject carCam;
    bool clickDown;

    public LayerMask Vehicle;

    // Start is called before the first frame update
    void Start()
    {
        player = this.gameObject;
        player.GetComponent<PlayerController>();
        playerCam = player.GetComponentInChildren<Transform>();

        carCam = GameObject.FindGameObjectWithTag("CarCam");
        carCam.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            clickDown = true;
        }
    }

    void FixedUpdate()
    {
        Debug.DrawRay(playerCam.position, player.transform.forward, Color.blue);
        if (clickDown)
        {
            clickDown = false;
            if (Physics.Raycast(playerCam.position, player.transform.forward, out RaycastHit hit, 1f, Vehicle))
            {
                CarController carController = hit.transform.GetComponentInParent<CarController>();

                carCam.SetActive(true);
                carCam.GetComponent<CarCam>().SwapCar(hit.transform);
                carCam.transform.position = playerCam.position;
                player.transform.parent = hit.transform;
                player.SetActive(false);
                carController.GetComponentInChildren<ElementEnable>().GetInCar.enabled = false;
                carController.GetComponent<GetOutOfCar>().enabled = true;
                carController.GetComponent<GetOutOfCar>().SetVars(player, carCam, carController);
            }
        }
    }
}
