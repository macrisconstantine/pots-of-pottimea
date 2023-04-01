using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    // Allows for setting of enemy health and damage
    [SerializeField] public float health = 100f;
    [SerializeField] public int damage = 1;
    GameObject gm;

    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM");
    }

    // Subtracts damaged done by player from total enemy health
    public void DamageEnemy(float damage)
    {
        health -= damage;

        // Kill enemy if health goes to 0
        if (health <= 0)
        {
            health = 0;
            //GetComponent<EnemyAI>().enabled = false;
            KillEnemy();
        }
    }

    // Deal the set amount of damage to the player when this function is called
    public void DamagePlayer()
    {
        if (gm != null)
        {
            gm.GetComponentInChildren<PlayerHealth>().DamagePlayer(damage);
        }
    }

    // Trigger effects of enemy death, disable collider, and destroy gameObject after a delay
    public void KillEnemy()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponentInChildren<Animator>().SetTrigger("death");
        Destroy(gameObject, 1f);
    }
}
