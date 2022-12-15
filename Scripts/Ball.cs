using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Rigidbody2D rb;
    private float timer;

    public AudioClip[] bounce;
    private AudioSource audioSource;

    public bool randomJumpOn;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    // Update is called once per frame
    void Update()
    {
        if (!randomJumpOn) return;

        timer += Time.deltaTime;

        if (timer > 2)
        {
            timer = 0;
            rb.velocity = RandomVector(3, 18);
        }
    }

    private Vector3 RandomVector(float min, float max)
    {
        var x = Random.Range(min, max);
        var y = Random.Range(min, max);
        return new Vector3(x, y, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var random = Random.Range(0, bounce.Length);
        audioSource.clip = bounce[random];
        audioSource.Play();
    }
}
