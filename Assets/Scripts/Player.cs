using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    public delegate void PlayerDownedHandler(Player playerDowned);
    public static event PlayerDownedHandler playerDownedEvent;
    
    public delegate void PlayerDiedHandler(Player playerDied);
    public static event PlayerDiedHandler playerDiedEvent;
    
    [SerializeField] protected int maxHp;
    [SerializeField] protected int currentHp;

    private bool isDowned = false;
    public void TakeDamage(int damage)
    {
        currentHp -= damage;

        if (currentHp <= 0)
        {
            if (isDowned)
            {
                // Si ya estabas tirado y te vuelven a hacer daño, sí te mueres
                Destroy(gameObject);
            }
            else
            {
                isDowned = true;
            }
        }
    }
    
    public void TakeDamageOwnClass(int damage)
    {
        currentHp -= damage;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Te suscribes al evento.
        // Quiere decir que cuando se haga el invoke de inflictDmamageEvent, se va a ejecutar la función
        // OnReceiveDamage de esta clase.
        BaseEnemy.inflictDamageEvent += OnReceiveDamage;
    }

    void OnReceiveDamage(object objectToDamage, int damage)
    {
        Debug.Log($"Entrando al OnReceiveDamage del gameObject: {gameObject.name}");
        if ((GameObject)objectToDamage == gameObject)
        {
            Debug.Log($"El player {gameObject.name} está tomando: {damage}" +
                      $"en su método OnReceiveDamage");

            // Si sí fue este gameObject el que recibió este daño, entonces se lo aplica.
            currentHp -= damage;
            if (currentHp <= 0)
            {
                if (isDowned)
                {
                    // Si ya estabas tirado y te vuelven a hacer daño, sí te mueres
                    Destroy(gameObject);
                    // Le avisa a todos los interesados que este player acaba de morir.
                    playerDiedEvent?.Invoke(this);
                }
                else
                {
                    isDowned = true;
                    // Le avisa a todos los interesados que este player está como downed.
                    playerDownedEvent?.Invoke(this);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
