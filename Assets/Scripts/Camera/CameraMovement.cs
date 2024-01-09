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

    Vector3 camPos;

    // Start is called before the first frame update
    void Start()
    {
        camOffset = transform.position - player.transform.position;
        camPos.z = transform.position.z;
    }

    private void FixedUpdate()
    {
        CalcCamPos();

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

        camPos.x = Mathf.Lerp(transform.position.x, player.position.x + camOffset.x, Time.deltaTime * camSmoothing.x);
        camPos.y = Mathf.Lerp(transform.position.y, player.position.y + camOffset.y, Time.deltaTime * camSmoothing.y);
    }
}
