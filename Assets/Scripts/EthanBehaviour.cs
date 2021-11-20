using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EthanBehaviour : MonoBehaviour
{
    [SerializeField]
    private Transform self;

    [System.NonSerialized]
    public bool isEnable = false;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void StartLiving()
    {
        isEnable = true;
        gameObject.SetActive(true);
        self.localPosition = new Vector3(0.0f, self.localPosition.y, 0.0f);
    }

    public void Die()
    {
        isEnable = false;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnable)
            self.Translate(0.1f * Time.deltaTime, 0.0f, 0.0f);
    }
}
