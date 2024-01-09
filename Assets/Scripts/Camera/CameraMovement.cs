using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] Transform player;

    [SerializeField] Vector3 camOffset; //offset between player position and camera position
    [SerializeField] Vector3 camDeadZone;
    [SerializeField] Vector3 camSmoothing; //how quickly does the camera follow the player

    Vector3 dir; //direction mouse is moving in
    float distFromPlayer; //distance mouse is from player

    Vector3 camPos; //final camera position

    // Start is called before the first frame update
    void Start()
    {
        camOffset = transform.position - player.transform.position;
        camPos.z = transform.position.z;
    }

    private void FixedUpdate()
    {
        CalcCamPos(); //calculate the camera position

        transform.position = camPos;
    }

    void CalcCamPos()
    {
        //get the mouse position and calculate the distance between the mouse position and the camera position
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        dir = (mousePos - transform.position).normalized;

        //camera position is approximatly equal to player position
        distFromPlayer = (mousePos - transform.position).magnitude;

        //this calculation makes it so that when the mouse is close to the player, the camera does not move very much
        //however, when the mouse is far away from the player on the screen, the camera moves a lot more and follows
        //the mouse position fairly closely
        camOffset.x = dir.x / (10 * (1 / distFromPlayer));
        camOffset.y = dir.y / (10 * (1 / distFromPlayer));
        camSmoothing = new Vector3(7f, 7f, 0);

        //update the camera position
        camPos.x = Mathf.Lerp(transform.position.x, player.position.x + camOffset.x, Time.deltaTime * camSmoothing.x);
        camPos.y = Mathf.Lerp(transform.position.y, player.position.y + camOffset.y, Time.deltaTime * camSmoothing.y);
    }
}
