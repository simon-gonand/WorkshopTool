using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Transform self;
    public Rigidbody selfRigidbody;

    private Vector2 movements = Vector2.zero;
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

    // Update is called once per frame
    void FixedUpdate()
    {
        selfRigidbody.velocity = new Vector3(movements.x, 0.0f, movements.y);
    }
}
