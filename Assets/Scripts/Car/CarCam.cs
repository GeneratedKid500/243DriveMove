using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCam : MonoBehaviour
{
    public Vector3 offset;
    public Transform target;
    //public float translateSpeed = 10;
    //public float rotationSpeed = 10;

    CarController carVars;
    Vector3 vel = Vector3.zero;
    Quaternion rotVel = Quaternion.identity;

    private void Awake()
    {
        //carVars = target.GetComponentInParent<CarController>();
        this.transform.parent = null;
    }

    void Update()
    {
        if (carVars != target.GetComponentInParent<CarController>())
        {
            carVars = target.GetComponentInParent<CarController>();
        }

        HandleTranslation();
        HandleRotation();
    }

    void HandleTranslation()
    {
        Vector3 targetPosition = target.TransformPoint(offset);
        Debug.DrawRay(transform.position, targetPosition - transform.position, Color.red);

        if (carVars.speedInput < 0)
        {
            targetPosition = target.TransformPoint(new Vector3(offset.x, offset.y, -offset.z));
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref vel, .15f);
    }

    void HandleRotation()
    {
        var direction = target.position - transform.position;
        var rotation = Quaternion.LookRotation(direction, Vector3.up);

        transform.rotation = SmoothDamp(transform.rotation, rotation, ref rotVel, .15f);
    }

    public void SwapCar(Transform hitCar)
    {
        target = hitCar;
        target.GetComponentInParent<CarController>().isDriven = true;
    }

    /// <summary>
    /// Quaternion Version of the Vector3 function SmoothDamp
    /// </summary>
    /// <param name="rot">starting Quaternion rotation</param>
    /// <param name="target">target Quaternion rotation</param>
    /// <param name="deriv"> The current velocity, modified by the function when called</param>
    /// <param name="time"> approx time it will take to reach the target</param>
    /// <returns></returns>
    private Quaternion SmoothDamp(Quaternion rot, Quaternion target, ref Quaternion deriv, float time)
    {
        if (Time.deltaTime < Mathf.Epsilon) return rot;
        // account for double-cover
        var Dot = Quaternion.Dot(rot, target);
        var Multi = Dot > 0f ? 1f : -1f;
        target.x *= Multi;
        target.y *= Multi;
        target.z *= Multi;
        target.w *= Multi;
        // smooth damp (nlerp approx)
        var Result = new Vector4(
            Mathf.SmoothDamp(rot.x, target.x, ref deriv.x, time),
            Mathf.SmoothDamp(rot.y, target.y, ref deriv.y, time),
            Mathf.SmoothDamp(rot.z, target.z, ref deriv.z, time),
            Mathf.SmoothDamp(rot.w, target.w, ref deriv.w, time)
        ).normalized;

        // ensure deriv is tangent
        var derivError = Vector4.Project(new Vector4(deriv.x, deriv.y, deriv.z, deriv.w), Result);
        deriv.x -= derivError.x;
        deriv.y -= derivError.y;
        deriv.z -= derivError.z;
        deriv.w -= derivError.w;

        return new Quaternion(Result.x, Result.y, Result.z, Result.w);

    }
}
