using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    public Vector3 target;
    private Rigidbody2D rb;
    public float force;
    public AudioClip[] death;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = GameManager.current.playerObject.transform.position;
        Debug.Log("target is: " + target);
        Vector3 direction = target - transform.position;
        rb.velocity = new Vector3(direction.x, direction.y).normalized * force;

        // float rot = Mathf.Atan2(-direction.y - direction.x) * Mathf.Rad2Deg;

        var offset = 90f;
        Vector2 rotDirection = target - transform.position;
        rotDirection.Normalize();
        float angle = Mathf.Atan2(rotDirection.y, rotDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(Vector3.forward * (angle + offset));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Ground")
        {
            Destroy(this.gameObject);
        }
    }
}
