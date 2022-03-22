using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseMoveSpeed = 10f;

    private Transform player;

    private float xAxisRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //player = gameObject.GetComponentInParent<Transform>(); //doesnt work bc fills space with this obj's transform since its first

        player = transform.parent.GetComponent<Transform>();

        //hide and lock cursor to center of screen:
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //set mouse input based on mouse:
        float mouseXinput = Input.GetAxis("Mouse X");
        float mouseYinput = Input.GetAxis("Mouse Y");

        //apply mouse sensitivity:
        mouseXinput *= mouseMoveSpeed;
        mouseYinput *= mouseMoveSpeed;

        //apply input independent of framerate (once everytime 'Update()' called) (don't need to mult by Time.deltaTime bc mouse input is already framerate independent)

        //decrease x axis's rotation by however much move mouse (needs to be - and not + for some reason):
        xAxisRotation -= mouseYinput;

        //only rotate cam up/down from feet to sky (cant over rotate and look behind player): 
        xAxisRotation = Mathf.Clamp(xAxisRotation, -90f, 90f);

        //rot camera by new x axis rot:
        transform.localRotation = Quaternion.Euler(xAxisRotation, 0f, 0f);

        //set player rotation based on mouse horizontal movement (dont use eulers?)
        player.Rotate(Vector3.up * mouseXinput); //roting around y-axis (so Vector3.up)
    }
}
