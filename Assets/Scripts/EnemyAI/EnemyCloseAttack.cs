using UnityEngine;

public class EnemyCloseAttack : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private int damage;
    [SerializeField] private AudioClip swordAirSound;

    //References
    private Animator anim;
    private Health health;
    private EnemyPatrol patrol;
    private Senses senses;

    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        patrol = GetComponentInParent<EnemyPatrol>();
        health = GetComponent<Health>();
        senses = GetComponent<Senses>();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;
        if (senses.ThreatInCloseRange())
        {
            Attack();
        }
        if (health.isDead())
        {
            patrol.enabled = false;
            this.enabled = false;
        }
    }

    public void Attack()
    {
        if (cooldownTimer >= attackCooldown)
        {
            anim.SetTrigger("closeAttack");
            SoundManager.instance.PlaySound(swordAirSound);
            cooldownTimer = 0;
        }
    }

    private void DamagePlayer()
    {
        var hit = senses.GetHitInCloseRange(Direction.straight);
        if (hit.collider != null)
        {
            Health playerHealth = hit.transform.GetComponent<Health>();
            playerHealth.TakeDamage(damage);
        }
    }
}