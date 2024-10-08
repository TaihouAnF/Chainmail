using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb;

    public event Action OnAttack;

    [Header("Enemy Attribute")]
    public float BootTime = 3.0f;
    public float Speed = 5.0f;
    public float CoolDown;
    public float ShootCoolDown = 2.0f;

    [Header("Reference")]
    public Centipede centipede;
    private CentipedeSection targetSection;
    public AudioSource soundShoot;
    public GameManager gameManager;

    private void Awake() 
    { 
        rb = GetComponent<Rigidbody2D>();
        targetSection = null;
        centipede.OnTargetDestroy += OnTargetClear;
    }

    void Start() 
    {
        BootTime = GameManager.Instance.currEnemyBootTime;
        Speed = GameManager.Instance.currEnemySpeed;
        CoolDown = GameManager.Instance.currEnemyCdTime;
        ShootCoolDown = CoolDown;
    }

    // Update is called once per frame
    void Update()
    { 
        if (BootTime <= 0) {
            UpdateMovement();
            UpdateAttack();
        }
        else 
        {
            BootTime -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Borrow from CentipedeSection, getting the position in the grid.
    /// </summary>
    /// <param name="position">Current position in 2D.</param>
    /// <returns>Accurate Integer position</returns>
    private Vector2 GridPosition(Vector2 position)
    {
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);
        return position;
    }

    /// <summary>
    /// Clear the target after destroy/destroy event raises.
    /// </summary>
    private void OnTargetClear() {
        if (targetSection != null)
        {
            targetSection = null;
        }
    }

    /// <summary>
    /// Enemy movement, following one of section of centipedes.
    /// </summary>
    private void UpdateMovement() 
    {
        if (centipede.sections.Count == 0) { return; }
        if (targetSection == null) 
        {
            targetSection = centipede.GetRandomSection();  // Only when new section required
            targetSection.isLockedOn = true;
        }
        Vector2 targetPos = GridPosition(targetSection.transform.position);
        Vector2 currPos = transform.position;
        targetPos.y = currPos.y;
        float currSpeed = Speed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(currPos, targetPos, currSpeed);
    }

    /// <summary>
    /// Attack Event.
    /// </summary>
    private void Attack() 
    {
        OnAttack?.Invoke();
        soundShoot.Play();

    }

    /// <summary>
    /// Enemy attack, triggering the attack event for observers.
    /// </summary>
    private void UpdateAttack() 
    {
        if (centipede.sections.Count == 0) { return; }
        if (ShootCoolDown <= 0) 
        {
            Attack();
            ShootCoolDown = CoolDown;
        }
        else 
        {
            ShootCoolDown -= Time.deltaTime;
        }
    }

    private void OnDestroy() 
    {
        if (centipede != null) 
        {
            centipede.OnTargetDestroy -= OnTargetClear;
        }
    }
}