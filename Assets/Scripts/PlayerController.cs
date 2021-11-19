using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Transform self;
    public Rigidbody selfRigidbody;
    public Transform cam;

    private Vector2 movements = Vector2.zero;
    private Vector2 cameraMovements = Vector2.zero;
    // Start is called before the first frame update
    void Start()
    {
        Transform spawn = GameObject.FindGameObjectWithTag("Player").transform;
        self.position = new Vector3(spawn.position.x, self.position.y, spawn.position.z);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movements = context.ReadValue<Vector2>();
    }

    public void OnCameraMove(InputAction.CallbackContext context)
    {
        cameraMovements += context.ReadValue<Vector2>();
        //cameraMovements.x = Mathf.Clamp(cameraMovements.x, -60f, 60f);
        //cameraMovements.y = Mathf.Clamp(cameraMovements.y, -60f, 60f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        selfRigidbody.velocity = new Vector3(movements.x, 0.0f, movements.y);
        self.eulerAngles = new Vector3(0.0f, cameraMovements.x, 0.0f);
        cam.eulerAngles = new Vector3(-cameraMovements.y, 0.0f, 0.0f);
    }
}
