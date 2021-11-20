using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Transform self;
    public Transform cam;
    public ParticleSystem bulletImpact;

    [Range(0, 1)]
    public float sensivityX;
    [Range(0, 1)]
    public float sensivityY;
    [Range(1, 10)]
    public float speed = 1.0f;

    private Vector2 movementInput = Vector2.zero;
    private Vector2 cameraMovements = Vector2.zero;
    private Vector3 camOriginalPos;
    
    // Start is called before the first frame update
    void Start()
    {
        Transform spawn = GameObject.FindGameObjectWithTag("Player").transform;
        self.position = new Vector3(spawn.position.x, self.position.y, spawn.position.z);
        camOriginalPos = cam.localPosition;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0.0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 999.0f))
        {
            bulletImpact.transform.position = hit.point;
            bulletImpact.transform.LookAt(self.position);
            if (hit.collider.CompareTag("Enemy"))
            {
                hit.collider.GetComponent<EthanBehaviour>().Die();
            }

            bulletImpact.Play();
            StartCoroutine(Recoil());
        }
    }

    public IEnumerator Recoil()
    {
        float time = 0.0f;
        while (time < 0.1f)
        {
            float zOffset = Random.Range(0.0f, 0.005f);
            cam.localPosition -= new Vector3(0.0f, 0.0f, zOffset);
            time += Time.deltaTime;

            yield return null;
        }

        cam.localPosition = camOriginalPos;    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnMouseMovedX(InputAction.CallbackContext context)
    {
        cameraMovements.x = context.ReadValue<float>() * sensivityX;
    }

    public void OnMouseMovedY(InputAction.CallbackContext context)
    {
        cameraMovements.y -= context.ReadValue<float>() * sensivityY;
        cameraMovements.y = Mathf.Clamp(cameraMovements.y, -60.0f, 60.0f);
    }

    // Update is called once per frame
    void Update()
    {
        self.Translate(new Vector3(movementInput.x, 0.0f, movementInput.y) * speed * Time.deltaTime);

        self.Rotate(Vector3.up * cameraMovements.x);
        cam.localRotation = Quaternion.Euler(cameraMovements.y, 0.0f, 0.0f);
    }
}
