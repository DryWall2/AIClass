using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 6.0f;
    public float sprintSpeed = 12f;
    public float jumpSpeed = 8.0f;
    public float gravity = -9.8f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = .4f;
    public LayerMask groundMask;

    private Vector3 velocity;
    private CharacterController controller;
    private bool isGrounded;
    private bool isSprinting;


    // New
    public int health = 100;
    public HealthDisplay healthDisplay;
    public GameObject Bullet;
    public Transform bulletSpawnPoint;
    public float curSpeed;
    private bool isDead = false;
    private GameManager gm;

    protected float shootRate;
    protected float fastShootRate = .1f;
    protected float elapsedTime;
    private bool isAttacking = false;

    public bool spinTime;
    public float spinTimeLeft;
    public bool isCrouching = false;

    public int bulletsLeft = 20;

    private Vector3 homePos;

    public bool canMove = false;


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();

        gm = GameObject.FindObjectOfType<GameManager>();
    }

    void OnEndGame()
    {
        // Don't allow any more control changes when the game ends

        this.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove == false)
            return;


        gm.menu.AmmoText.text = "Ammo: " + bulletsLeft;

        elapsedTime += Time.deltaTime;


        UpdateWeapon();

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;



        if (Input.GetKey(KeyCode.LeftShift))
        {
            controller.Move(moveDirection * sprintSpeed * Time.deltaTime);
            isSprinting = true;
        }
        else
        {
            controller.Move(moveDirection * speed * Time.deltaTime);
            isSprinting = false;
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            isCrouching = true;
        }
        else
        {
            isCrouching = false;
            
        }



        velocity.y += gravity * Time.deltaTime;
        curSpeed = moveDirection.x + moveDirection.y + moveDirection.z;

        controller.Move(velocity * Time.deltaTime);
    }


    void UpdateWeapon()
    {
        if (Input.GetMouseButtonDown(0) && bulletsLeft > 0)
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
                bulletsLeft -= 1;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("I have been hit by an object with the tag: " + collision.transform.tag);
        //Reduce health
        if (collision.gameObject.tag == "Bullet" && !spinTime)
        {
            Debug.Log("I have been hit by a bullet");

            health -= collision.gameObject.GetComponent<Bullet>().damage;
            healthDisplay.SetHealth(health);

            if (health < 1)
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
    }
}
