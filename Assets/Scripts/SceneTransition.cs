using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string scemnel;

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Player":
            case "VehicleCollider":
                SceneManager.LoadScene(scemnel);
                break;
        }

    }
}
