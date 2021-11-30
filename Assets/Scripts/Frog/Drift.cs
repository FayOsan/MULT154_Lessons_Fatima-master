using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;

public class Drift : MonoBehaviour
{
    public float speed = 5.0f;

    public enum DriftDerection
    {
        LEFT = -1,
        RIGHT = 1
    }
    public DriftDerection driftDerection = DriftDerection.LEFT;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(driftDerection)
        {
            case DriftDerection.LEFT:
                transform.Translate(Vector3.left * Time.deltaTime * speed);
                break;
            case DriftDerection.RIGHT:
                transform.Translate(Vector3.right * Time.deltaTime * speed);
                break;
        }

        if(transform.position.x < -80 || transform.position.x > 80)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            GameObject child = collision.gameObject;
            child.transform.SetParent(gameObject.transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            GameObject child = collision.gameObject;
            child.transform.SetParent(null);
        }
    }
}
