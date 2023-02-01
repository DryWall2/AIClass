using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    //Explosion Effect
    public GameObject Explosion;
    Rigidbody rigidbody;

    public float Speed = 600.0f;
    public float LifeTime = 3.0f;
    public int damage = 20;

    void Start()
    {
        //rigidbody = GetComponent<Rigidbody>();
        //rigidbody.AddForce(transform.forward * Speed, ForceMode.Impulse);
        Destroy(gameObject, LifeTime);
    }

    void Update()
    {
        transform.position += transform.forward * Speed * Time.deltaTime;
        //rigidbody.velocity = new Vector3(transform.forward.x, transform.up.y * -.25f, transform.forward.z) * Speed * Time.deltaTime;
        
        
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Bullet Collision: " + collision.transform.name);

        if(collision.transform.tag == "Player" && this.transform.tag != "PBullet")
        {

            Destroy(gameObject);
        }
        if(collision.transform.tag != "Tree")
        {
            Destroy(gameObject);

        }

    }
}