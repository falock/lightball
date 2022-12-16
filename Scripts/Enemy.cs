using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Monetization;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Info")]
    // enemy turns around every now and then - coroutine with timeer
    public int turnTime = 2;
    // how quickly can this enemy shoot at the player?
    public float fireRate = 3f;
    // can the enemy flip?
    public bool flip;
    public GameObject bullet;
    private GameObject player;
    public Transform bulletSpawnPoint;

    [Header("Assign in Inspector")]
    [SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private GameObject coinPrefab;

    public float reach = 2;
    public bool inRange = false;
    public Transform currentTarget;
    private int i = 0;
    public bool shootingPlayer;


    public float nextFire;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        nextFire = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.current.gamePaused) return;

        if (!flip && !inRange)
        {
            StopAllCoroutines();
            flip = true;
            StartCoroutine(Countdown());
        }
        
        // if the player is in range, wait to shoot the player
        if(inRange)
        {
            StopAllCoroutines();
            timer += Time.deltaTime;

            if (timer > 1)
            {
                timer = 0;
                ShootPlayer();
            }
        }
        

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // if ball, take damage
        if (other.tag == "Ball")
        {
           TakeDamage();
        }
    }

    void TakeDamage()
    {
        var pe = Instantiate(deathParticles, gameObject.transform.position, gameObject.transform.rotation);
        var coin = Instantiate(coinPrefab, gameObject.transform.position, gameObject.transform.rotation);
        gameObject.SetActive(false);
    }

    void ShootPlayer()
    {
        //shootingPlayer = true;
        //GameObject Bullet = Instantiate(bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        //Bullet.GetComponent<ProjectileMovement>().target = player.transform.position;
        //Bullet.GetComponent<Rigidbody2D>().velocity = (player.transform.position - bullet.transform.position).normalized * 5f;
        //Rigidbody2D rb = Bullet.GetComponent<Rigidbody2D>();
        ///b.AddForce(bulletSpawnPoint.up * 2, ForceMode2D.Impulse);
        //nextFire = Time.time + fireRate;
        // shootingPlayer = false;

        var bullet2 = Instantiate(bullet, bulletSpawnPoint.position, Quaternion.identity);

    }

    void Flip()
    {
        if (transform.rotation.y == 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0); // Left
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0); // Right
        }
    }

    IEnumerator Countdown()
    {
        int counter = turnTime;
        while (counter > 0 && !inRange)
        {
            yield return new WaitForSeconds(1);
            counter--;
        }
        Flip();
        flip = false;
        StopCoroutine(Countdown());
    }
}
