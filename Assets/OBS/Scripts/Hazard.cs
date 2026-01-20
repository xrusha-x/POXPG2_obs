using UnityEngine;
using System.Collections.Generic;

public class Hazard : MonoBehaviour
{
    public int damageAmount = 1;
    public float attackCooldown = 0.7f;
    private Dictionary<PlayerHealth, float> nextAvailableHit = new Dictionary<PlayerHealth, float>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryDamage(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryDamage(collision.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TryDamage(collision.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryDamage(collision.gameObject);
    }

    private void TryDamage(GameObject obj)
    {
        if (obj == null) return;
        PlayerHealth playerHealth = obj.GetComponent<PlayerHealth>();
        if (playerHealth == null) playerHealth = obj.GetComponentInParent<PlayerHealth>();
        if (playerHealth == null) return;

        float now = Time.time;
        float nextTime = 0f;
        if (nextAvailableHit.TryGetValue(playerHealth, out nextTime))
        {
            if (now < nextTime) return;
        }
        nextAvailableHit[playerHealth] = now + attackCooldown;
        playerHealth.TakeDamage(damageAmount);
    }
}
