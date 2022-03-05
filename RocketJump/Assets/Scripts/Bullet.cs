using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    private CircleCollider2D bulletCollider;
    private Rigidbody2D bulletRigidbody;

    private Rigidbody2D playerRigidbody;

    Vector2 firstDirection, lastVelocity;
    float speed = 15f;

    private ParticleSystem particle;
    private SoundSystem soundSystem;

    public void Initialize(Vector2 firstDirection, Rigidbody2D playerRigidbody, ParticleSystem particle, SoundSystem soundSystem) {
        bulletCollider = GetComponent<CircleCollider2D>();
        bulletRigidbody = GetComponent<Rigidbody2D>();

        this.soundSystem = soundSystem;

        this.firstDirection = firstDirection;
        this.playerRigidbody = playerRigidbody;
        this.particle = particle;

        bulletRigidbody.velocity = firstDirection.normalized * speed;

        Destroy(gameObject, 2f);
    }

    private void FixedUpdate() {
        lastVelocity = bulletRigidbody.velocity;
        if(lastVelocity.magnitude <= speed - 5) {
            Vector2 changeDirection = Vector2.Perpendicular(firstDirection);
            bulletRigidbody.velocity = -changeDirection.normalized * speed;
        }
        if(lastVelocity.magnitude <= 5) {
            Destroy(gameObject);
        }
    }

    private void OnDestroy() {
        particle.ShowParticle(1, transform.position);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Platform") || collision.gameObject.layer == LayerMask.NameToLayer("Slope") || collision.gameObject.layer == LayerMask.NameToLayer("Bullet")) {
            Vector3 direction = Vector3.Reflect(lastVelocity.normalized, collision.contacts[0].normal);

            bulletRigidbody.velocity = direction * speed;

            particle.ShowParticle(1, transform.position);
        } else if(collision.gameObject.layer == LayerMask.NameToLayer("Player")) {
            Vector3 direction = -lastVelocity.normalized;

            bulletRigidbody.velocity = direction * speed;

            playerRigidbody.velocity = Vector2.zero;
            playerRigidbody.AddForce(-direction * 800f);

            particle.ShowParticle(1, transform.position);
            soundSystem.PlaySound(0);
        } else if(collision.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "Slime") {
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if(collision.gameObject.tag == "Player") {
            bulletCollider.isTrigger = false;
        }
    }
}
