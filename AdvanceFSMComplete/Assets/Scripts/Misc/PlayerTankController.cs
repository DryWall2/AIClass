using UnityEngine;
using System.Collections;
using TMPro;

public class PlayerTankController : MonoBehaviour
{
    private CharacterController CharacterController;
    public GameObject Bullet;
    private Transform bulletSpawnPoint;
    public float curSpeed, targetSpeed, rotSpeed;
    private float turretRotSpeed = 10.0f;
    private float maxForwardSpeed = 300.0f;
    private float maxBackwardSpeed = -300.0f;

    //Tank Health
    private int health = 100;
    public HealthDisplay healthDisplay;
    private bool isDead = false;
    private GameManager gm;

    //Bullet shooting rate
    protected float shootRate;
    protected float fastShootRate = .1f;
    protected float elapsedTime;
    private bool isAttacking = false;

    public bool spinTime;
    public float spinTimeLeft;
    public bool isCrouching = false;

    private Transform Turret;
    private Vector3 homePos;

    //TODO: add stealth mode

    void Start()
    {
        CharacterController = GetComponent<CharacterController>();
        //Tank Settings
        rotSpeed = 150;


        gm = GameObject.FindObjectOfType<GameManager>();

        Turret = gameObject.transform.GetChild(1).transform;
        bulletSpawnPoint = Turret.transform.GetChild(0).transform;

        homePos = Turret.position;
    }

    void OnEndGame()
    {
        // Don't allow any more control changes when the game ends
        this.enabled = false;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (!isDead)
        {
            UpdateControl();
            UpdateWeapon();
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            isCrouching = true;
        }
        else
        {
            isCrouching = false;
        }


        

    }
    
    void UpdateControl()
    {
        //AIMING WITH THE MOUSE
        // Generate a plane that intersects the transform's position with an upwards normal.
        Plane playerPlane = new Plane(Vector3.up, transform.position + new Vector3(0, 0, 0));

        // Generate a ray from the cursor position
        Ray RayCast = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Determine the point where the cursor ray intersects the plane.
        float HitDist = 0;

        // If the ray is parallel to the plane, Raycast will return false.
        /*if (playerPlane.Raycast(RayCast, out HitDist) && !spinTime)
        {
            
            // Get the point along the ray that hits the calculated distance.
            Vector3 RayHitPoint = RayCast.GetPoint(HitDist);

            Quaternion targetRotation = Quaternion.LookRotation(RayHitPoint - transform.position);
            Turret.transform.rotation = Quaternion.Slerp(Turret.transform.rotation, targetRotation, Time.deltaTime * turretRotSpeed);
            //this.transform.rotation = targetRotation;
        }
        else */
        if (spinTime)
        {
            if(spinTimeLeft < 0) { spinTime = false; spinTimeLeft = 3f; Turret.position = homePos; return; }
            spinTimeLeft -= Time.deltaTime;
            Turret.transform.Rotate(0, 150 * Time.deltaTime, 0);
            if (elapsedTime >= fastShootRate)
            {
                //Reset the time
                elapsedTime = 0.0f;

                //Also Instantiate over the PhotonNetwork
                Instantiate(Bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
                //Instantiate(Bullet, Turret.position, Turret.rotation);
                isAttacking = false;
            }
            
        }


        if (isCrouching)
        {
            maxBackwardSpeed = -150f;
            maxForwardSpeed = 150f;
        }
        else
        {
            maxForwardSpeed = 300f;
            maxBackwardSpeed = -300f;
        }

        if (Input.GetKey(KeyCode.W))
        {
            targetSpeed = maxForwardSpeed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            targetSpeed = maxBackwardSpeed;
        }
        else
        {
            targetSpeed = 0;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, -rotSpeed * Time.deltaTime, 0.0f);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, rotSpeed * Time.deltaTime, 0.0f);
        }

        

        //Determine current speed
        curSpeed = Mathf.Lerp(curSpeed, targetSpeed, 7.0f * Time.deltaTime);

        CharacterController.Move(transform.forward * Time.deltaTime * curSpeed);

    }
    void UpdateWeapon()
    {
        if(Input.GetMouseButtonDown(0))
        {
            isAttacking = true;
            if (elapsedTime >= shootRate)
            {
                //Reset the time
                elapsedTime = 0.0f;

                //Also Instantiate over the PhotonNetwork
                Instantiate(Bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
                //Instantiate(Bullet, Turret.position, Turret.rotation);
                isAttacking = false;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //Reduce health
        if (collision.gameObject.tag == "Bullet" && !spinTime)
        {
            health -= collision.gameObject.GetComponent<Bullet>().damage;
            healthDisplay.SetHealth(health);

            if(health < 1)
            {
                if (!isDead)
                {
                    isDead = true;
                    Explode();
                }
            }
        }

    }

    protected void Explode()
    {
        gm.EnableEndOfGame(false);

        float rndX = Random.Range(10.0f, 30.0f);
        float rndZ = Random.Range(10.0f, 30.0f);
        for (int i = 0; i < 3; i++)
        {
            GetComponent<Rigidbody>().AddExplosionForce(1000.0f, transform.position - new Vector3(rndX, 10.0f, rndZ), 40.0f, 10.0f);
            GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(rndX, 20.0f, rndZ));
        }

        Destroy(gameObject, 1.5f);
    }
}