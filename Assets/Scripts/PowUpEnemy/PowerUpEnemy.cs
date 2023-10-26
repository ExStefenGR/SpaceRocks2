using UnityEngine;

public enum PowerUpType
{
    DiagonalShooting, // Red
    Forcefield,       // Blue
    LifeUp            // Green
}

public class PowerUpEnemy : Enemy
{
    [Header("Particle System")]
    [SerializeField] private ParticleSystem particles;

    [Header("Power-Up Gradients")]
    [SerializeField] private Gradient diagonalShootingGradient;
    [SerializeField] private Gradient forcefieldGradient;
    [SerializeField] private Gradient lifeUpGradient;

    private PowerUpType powerUpType;

    private new void Start()
    {
        canShoot = false;
        base.Start();
        // Randomly choose a power-up type for this instance
        powerUpType = (PowerUpType)Random.Range(0, System.Enum.GetValues(typeof(PowerUpType)).Length);

        // Get the color over lifetime module from the particle system
        ParticleSystem.ColorOverLifetimeModule col = particles.colorOverLifetime;

        // Set the gradient based on the power-up type
        switch (powerUpType)
        {
            case PowerUpType.DiagonalShooting:
                col.color = new ParticleSystem.MinMaxGradient(diagonalShootingGradient);
                break;
            case PowerUpType.Forcefield:
                col.color = new ParticleSystem.MinMaxGradient(forcefieldGradient);
                break;
            case PowerUpType.LifeUp:
                col.color = new ParticleSystem.MinMaxGradient(lifeUpGradient);
                break;
        }
    }

    private void FixedUpdate()
    {
        ProcessInput();
        Move();
        Animate();
    }

    protected override void ProcessInput()
    {
        MoveInLine();
    }

    protected override void OnDestroyed()
    {
        Debug.Log("OnDestroyed Triggered");  // This will print when OnDestroyed is called
                                             // Find the player
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            Player player = playerObject.GetComponent<Player>();
            Debug.Log("Player Found");  // This will print when the player is found
                                        // Activate the power-up
            player.ActivatePowerUp(powerUpType);
        }
        else
        {
            Debug.Log("Player not found"); // This will print if the player is not found
        }
    }
}
