using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


public abstract class PlayerState
{
    protected NetworkBehaviour thisObject;
    protected string stateName;
    protected GameObject player;
    protected PlayerState(NetworkBehaviour thisObj)
    {
        thisObject = thisObj;
        player = thisObject.gameObject;
    }

    public abstract void Start();
    public abstract void Update();
    public abstract void FixedUpdate();
    public abstract void OnCollisionEnter(Collision other);
    public abstract void OnTriggerExit(Collider other);
    public abstract void OnTriggerEnter(Collider other);

}

public class RiverState : PlayerState
{
    private Rigidbody rbPlayer;
    private Vector3 direction = Vector3.zero;
    public float speed = 20.0f;
    public GameObject[] spawnPoints = null;
    public RiverState(NetworkBehaviour thisObj) : base(thisObj)
    {
        stateName = "RiverLevel";
        GameData.gamePlayStart = Time.time;
    }
    // Start is called before the first frame update
    public  override void Start()
    {

        rbPlayer = player.GetComponent<Rigidbody>();
        spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");

    }



    // Update is called once per frame
    public override void Update()
    {

        float horMove = Input.GetAxis("Horizontal");
        float verMove = Input.GetAxis("Vertical");

        direction = new Vector3(horMove, 0, verMove);

    }

    public override void FixedUpdate()
    {

        rbPlayer.AddForce(direction * speed, ForceMode.Force);

        if (player.transform.position.z > 40)
        {
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, 40);
        }
        else if (player.transform.position.z < -40)
        {
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -40);
        }
    }

    private void Respawn()
    {
        int index = 0;
        while (Physics.CheckBox(spawnPoints[index].transform.position, new Vector3(1.5f, 1.5f, 1.5f)))
        {
            index++;
        }
        rbPlayer.MovePosition(spawnPoints[index].transform.position);
        rbPlayer.velocity = Vector3.zero;
    }

    public override void OnCollisionEnter(Collision other)
    {
        throw new System.NotImplementedException();
    }
    public override void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Exit"))
        {
            NetworkManager networkManager = 
                GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
            networkManager.ServerChangeScene("ForestLevel");
        }
    }


    public override void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Hazard"))
        {
            Respawn();
        }
    } 
}

public class ForestState : PlayerState
{
    public float speed = 120.0f;
    public float rotationSpeed = 10.0f;
    Rigidbody rgBody = null;
    float trans = 0;
    float rotate = 0;
    private Animator anim;
    private Camera camera;
    private Transform lookTarget;

    public delegate void DropHive(Vector3 pos);
    public static event DropHive DroppedHive;
    public ForestState(NetworkBehaviour thisObj) : base(thisObj)
    {
        stateName = "ForestLevel";
    }

    public override void Start()
    {
        player.transform.position = new Vector3(-20, 0.5f, -10);

        Transform rabbit = player.transform.Find("Rabbit");
        rabbit.transform.localEulerAngles = Vector3.zero;
        rabbit.transform.localScale = Vector3.one;
        rgBody = player.GetComponent<Rigidbody>();
        anim = player.GetComponentInChildren<Animator>();
        camera = player.GetComponentInChildren<Camera>();
        camera.enabled = true;
        lookTarget = GameObject.Find("HeadAimTarget").transform;
    }
    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DroppedHive?.Invoke(player.transform.position + (player.transform.forward * 10));
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

    public override void FixedUpdate()
    {
        Vector3 rot = player.transform.rotation.eulerAngles;
        rot.y += rotate * rotationSpeed * Time.deltaTime;
        rgBody.MoveRotation(Quaternion.Euler(rot));
        rotate = 0;

        Vector3 move = player.transform.forward * trans * speed;
        move.y = rgBody.velocity.y;
        rgBody.velocity = move * Time.deltaTime;

        trans = 0;
    }

    /*public override void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Hazard"))
        {
            anim.SetTrigger("died");
            //thisObject.StartCoroutine(ZoomOut());
        }
        else
        {
            anim.SetTrigger("twitchLeftEar");
        }
    }*/
    IEnumerator ZoomOut()
    {
        const int ITERATIONS = 25;
        for (int z = 0; z < 10; z++)
        {
            camera.transform.Translate(camera.transform.forward * -1 * 15.0f / ITERATIONS);
            yield return new WaitForSeconds(1.0f / ITERATIONS);
        }
    }

    public override void OnCollisionEnter(Collision other)
    {
        
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hazard"))
        {
            lookTarget.position = other.transform.position;
            thisObject.StartCoroutine(LookAndLookAway(lookTarget.position, other.transform.position));
        }

        if (other.CompareTag("Exit"))
        {
            NetworkManager networkManager =
                GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
            networkManager.ServerChangeScene("EndScene");
        }
    }

    public override void OnTriggerExit(Collider other)
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator LookAndLookAway(Vector3 targetpos, Vector3 hazardPos)
    {
        Vector3 targetDir = targetpos - player.transform.position;
        Vector3 hazardDir = hazardPos - player.transform.position;

        float angle = Vector2.SignedAngle(new Vector2(targetpos.x, targetpos.z), new Vector2(hazardPos.x, hazardPos.z));

        const int INTERVALS = 20;
        const float INTERVAL = 0.5f / INTERVALS;

        float angleInterval = angle / INTERVALS;

        for (int i = 0; i < INTERVALS; i++)
        {
            lookTarget.RotateAround(player.transform.position, Vector3.up, -angleInterval);
            yield return new WaitForSeconds(INTERVAL);
        }

        for (int i = 0; i < INTERVALS; i++)
        {
            lookTarget.RotateAround(player.transform.position, Vector3.up, -angleInterval);
            yield return new WaitForSeconds(INTERVAL);
        }
    }
}

public class PlayerContext : NetworkBehaviour
{
    PlayerState currentState;
    // Start is called before the first frame update
    void Start()
    {
        if (!isLocalPlayer) return;

        if(SceneManager.GetActiveScene().name == "RiverLevel")
        {
            currentState = new RiverState(this);
        }

        else if (SceneManager.GetActiveScene().name == "ForestLevel")
        {
            currentState = new ForestState(this);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
        if (currentState != null)
        {
            currentState.Start();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        currentState.Update();

    }
    void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        currentState.FixedUpdate();

    }

    void OnCollisonEnter(Collision collision)
    {
        if (!isLocalPlayer) return;

        currentState.OnCollisionEnter(collision);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isLocalPlayer) return;

        currentState.OnTriggerEnter(other);
    }

    void OnTriggerExit(Collider other)
    {
        if (!isLocalPlayer) return;

        currentState.OnTriggerExit(other);
    }
}
