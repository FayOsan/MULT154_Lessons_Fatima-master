using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavPlayerMovement : MonoBehaviour
{
    public float speed = 60.0f;
    public float rotationSpeed = 10.0f;
    Rigidbody rgBody = null;
    float trans = 0;
    float rotate = 0;
    private Animator anim;
    private Camera camera;

    public delegate void DropHive(Vector3 pos);
    public static event DropHive DroppedHive;

    private void Start()
    {
        rgBody = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        camera = GetComponentInChildren<Camera>();
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            DroppedHive?.Invoke(transform.position + (transform.forward * 10));
        }
        // Get the horizontal and vertical axis.
        // By default they are mapped to the arrow keys.
        // The value is in the range -1 to 1
        float translation = Input.GetAxis("Vertical");
        float rotation = Input.GetAxis("Horizontal");

        anim.SetFloat("speed", translation);

        trans += translation;
        rotate += rotation;
    }

    private void FixedUpdate()
    {
        Vector3 rot = transform.rotation.eulerAngles;
        rot.y += rotate * rotationSpeed * Time.deltaTime;
        rgBody.MoveRotation(Quaternion.Euler(rot));
        rotate = 0;

        Vector3 move = transform.forward * trans * speed;
        move.y = rgBody.velocity.y;
        rgBody.velocity = move * Time.deltaTime;

        trans = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Hazard"))
        {
            anim.SetTrigger("died");
            StartCoroutine(ZoomOut());
        }
        else
        {
            anim.SetTrigger("twitchLeftEar");
        }
    }

    IEnumerator ZoomOut()
    {
        const int ITERATIONS = 25;
        for(int z = 0; z < 10; z++)
        {
            camera.transform.Translate(camera.transform.forward * -1 * 15.0f / ITERATIONS);
            yield return new WaitForSeconds(1.0f / ITERATIONS);
        }
    }
}
