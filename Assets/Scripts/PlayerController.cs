using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Transform self;
    public Transform cam;
    public Collider selfCollider;
    public ParticleSystem bulletImpact;

    [Range(0, 1)]
    public float sensivityX;
    [Range(0, 1)]
    public float sensivityY;
    [Range(1, 10)]
    public float speed = 1.0f;
    public float fireRate = 1.0f;

    private Vector2 movementInput = Vector2.zero;
    private Vector2 cameraMovements = Vector2.zero;
    private Vector3 camOriginalPos;

    private float nextFire = 0.0f;

    public static PlayerController instance;

    private void Awake()
    {
        if (instance) Destroy(this.gameObject);
        else instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Transform spawn = GameObject.FindGameObjectWithTag("Player").transform;
        self.position = new Vector3(spawn.position.x, self.position.y, spawn.position.z);
        camOriginalPos = cam.localPosition;
        Cursor.visible = false;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (!GameManager.instance.isEndGame && nextFire < Time.time) {
            // Define a ray from the screen's centre
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0.0f));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 999.0f))
            {
                // If there is an Ethan => kill him
                if (hit.collider.CompareTag("Enemy"))
                {
                    hit.collider.GetComponent<EthanBehaviour>().Die();
                }

                // Play FX
                bulletImpact.transform.position = hit.point;
                bulletImpact.transform.LookAt(self.position);
                bulletImpact.Play();
                StartCoroutine(Recoil());
            }
            nextFire = Time.time + fireRate;
        }
    }

    public IEnumerator Recoil()
    {
        float time = 0.0f;
        while (time < 0.1f)
        {
            // Set a little recoil camera effect
            float zOffset = Random.Range(0.0f, 0.005f);
            cam.localPosition -= new Vector3(0.0f, 0.0f, zOffset);
            time += Time.deltaTime;

            yield return null;
        }

        cam.localPosition = camOriginalPos;    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!GameManager.instance.isEndGame)
            movementInput = context.ReadValue<Vector2>();
    }

    public void OnMouseMovedX(InputAction.CallbackContext context)
    {
        if (!GameManager.instance.isEndGame)
            cameraMovements.x = context.ReadValue<float>() * sensivityX;
    }

    public void OnMouseMovedY(InputAction.CallbackContext context)
    {
        if (!GameManager.instance.isEndGame)
        {
            cameraMovements.y -= context.ReadValue<float>() * sensivityY;
            cameraMovements.y = Mathf.Clamp(cameraMovements.y, -60.0f, 60.0f);
        }
    }

    public void OnReloadScene(InputAction.CallbackContext context)
    {
        GameManager.instance.RestartGame();
    }

    // Update is called once per frame
    void Update()
    {
        // Update player movements
        self.Translate(new Vector3(movementInput.x, 0.0f, movementInput.y) * speed * Time.deltaTime);

        // Update camera rotation
        self.Rotate(Vector3.up * cameraMovements.x);
        cam.localRotation = Quaternion.Euler(cameraMovements.y, 0.0f, 0.0f);
    }
}
