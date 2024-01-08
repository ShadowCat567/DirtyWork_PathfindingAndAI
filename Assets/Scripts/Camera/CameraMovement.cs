using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] Transform player;

    [SerializeField] Vector3 camOffset;
    [SerializeField] Vector3 camDeadZone;
    [SerializeField] Vector3 camSmoothing;

    Vector3 dir;
    float distFromPlayer;

    bool isShaking = false;
    [SerializeField] float shakeStrength = 0.5f;
    [SerializeField] float shakeDur = 0.15f;

    Vector3 camPos;

    public bool usingLockingCam { get; set; }
    //[SerializeField] LockingCamera lockCam;

    // Start is called before the first frame update
    void Start()
    {
        camOffset = transform.position - player.transform.position;
        camPos.z = transform.position.z;
        usingLockingCam = false;
    }

    private void FixedUpdate()
    {
        CalcCamPos();
        /*
        if (usingLockingCam)
        {
            //Debug.Log("locked camera");
            camPos.x = Mathf.Clamp(camPos.x, lockCam.getCurBounds().lowerBounds.position.x, lockCam.getCurBounds().upperBounds.position.x);
            camPos.y = Mathf.Clamp(camPos.y, lockCam.getCurBounds().lowerBounds.position.y, lockCam.getCurBounds().upperBounds.position.y);
        }
        */

        transform.position = camPos;
    }

    void CalcCamPos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        dir = (mousePos - transform.position).normalized;

        distFromPlayer = (mousePos - transform.position).magnitude;

        camOffset.x = dir.x / (10 * (1 / distFromPlayer));
        camOffset.y = dir.y / (10 * (1 / distFromPlayer));
        camSmoothing = new Vector3(7f, 7f, 0);

        if (isShaking)
        {
            //Debug.Log("I am shaking");
            camOffset += Random.insideUnitSphere * shakeStrength;
        }

        camPos.x = Mathf.Lerp(transform.position.x, player.position.x + camOffset.x, Time.deltaTime * camSmoothing.x);
        camPos.y = Mathf.Lerp(transform.position.y, player.position.y + camOffset.y, Time.deltaTime * camSmoothing.y);
    }

    public void ShakeCamera(float shakeStr = 0.5f)
    {
        shakeStrength = shakeStr;
        StartCoroutine(Shaking());
    }

    IEnumerator Shaking()
    {
        isShaking = true;
        yield return new WaitForSeconds(shakeDur);
        isShaking = false;
    }
}
