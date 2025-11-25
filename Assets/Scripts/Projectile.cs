using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] protected LayerMask desiredCollisions;
    [SerializeField] protected int damage;
    [SerializeField] protected float movementSpeed;
    [SerializeField] protected Rigidbody rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // cuando choca contra algo de lo que le interesa hace daño y/o se destruye
    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
            // si choca con algo se destruye.
            Destroy(gameObject);
        }
    }

    public virtual void Shoot()
    {
        rb.linearVelocity = movementSpeed * transform.forward;
    }
}
