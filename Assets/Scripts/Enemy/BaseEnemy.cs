using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

[RequireComponent(typeof(NavMeshAgent), typeof(Senses))]
public class BaseEnemy : MonoBehaviour, IDamageable
{
    // public delegate void EnemyToPlayerCollision(BaseEnemy enemy);
    
    
    public delegate void InflictDamageHandler(object objectToDamage, int damage);
    public static event InflictDamageHandler inflictDamageEvent;
    
    
    // HP
    [SerializeField] protected int maxHp;
    [SerializeField] protected int currentHp;
    
    // Ataques y daño
    [SerializeField] protected int contactDamage;
    [SerializeField] protected int attackDamage;
    // Cuántas veces por segundo ataca este enemigo.
    [SerializeField] protected float attackRate;
    [SerializeField] protected float attackRange;

    [SerializeField] protected float movementSpeed;

    // Sistema que se encarga detectar cosas importantes para el agente.
    protected Senses SensesSystem;

    protected NavMeshAgent NavAgent;

    public enum EDamageMethodType
    {
        DirectClass,
        IDamageableInterface,
        Events
    }

    public EDamageMethodType currentDamageMethodType = EDamageMethodType.DirectClass;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        if (NavAgent == null)
        {
            Debug.LogError($"El gameObject {gameObject.name} falló en obtener su navMeshAgent. Favor de verificar");
        }
        SensesSystem = GetComponent<Senses>();
        if (SensesSystem == null)
        {
            Debug.LogError($"El gameObject {gameObject.name} falló en obtener su Senses. Favor de verificar");
        }
    }

    // Evento que se manda a llamar cuando se colisiona con un collider que no es trigger.
    protected virtual void OnCollisionEnter(Collision collision)
    {
        // Chocar contra player
        // if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        if (collision.gameObject.layer == Constants.Layers.PlayerLayer)
        {
            switch (currentDamageMethodType)
            {
                case EDamageMethodType.DirectClass:
                    Debug.Log($"Causando: {contactDamage} de daño al player {collision.gameObject.name} " +
                              $"con el método de clase directa");
                    // Obtenemos el componente BasePlayer del gameObject con quien chocamos.
                    Player player = collision.gameObject.GetComponent<Player>();
                    if (player == null)
                    {
                        Debug.LogWarning($"El gameObject {collision.gameObject} en la layer de player no tiene su componente de base player");
                        return;
                    }
                    // Si sí tiene ese script, pues le hacemos daño.
                    player.TakeDamage(contactDamage);
                    break;
                case EDamageMethodType.IDamageableInterface:
                    Debug.Log($"Causando: {contactDamage} de daño al player {collision.gameObject.name}" +
                              $"a través del método de la interfaz");
                    IDamageable myPlayer = collision.gameObject.GetComponent<IDamageable>();
                    if (myPlayer == null)
                    {
                        Debug.LogWarning($"El gameObject {collision.gameObject} en la layer de player no tiene su componente de IDamageable");
                        return;
                    }
                    // Si sí tiene ese script, pues le hacemos daño.
                    myPlayer.TakeDamage(contactDamage);
                    break;
                case EDamageMethodType.Events:
                    Debug.Log($"Invocando el evento para causar: {contactDamage} de daño al player " +
                              $"{collision.gameObject.name}");
                    inflictDamageEvent?.Invoke(collision.gameObject, contactDamage);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            // Si fuera necesario, checar la tag de ese GameObject que está en la layer de player.
            // Normalmente, cuando toque al player le va a hacer daño por contacto.

            

            
            // Eventos y/o delegates
            // Publisher (el que produce/desencadena/genera el evento)
            // subscribers (a los que les interesa hacer algo cuando sucede el evento)
            // Publicar el evento "EnemyToPlayerCollision"
            // Probablemente a los players les interesa este evento.
            // Al recibir este evento, el player que chocó recibiría daño
            // Al sistema de Scoreboard (tabla de puntuaciones) también le podría interesar este evento
            // para actualizar cuánto daño han tomado los players por culpa de los enemigos.

        }
        
        // Chocar contra disparo o ataque del player
        
        // Paredes
        
        // Obstáculos
    }
    
    // Evento que se manda a llamar cuando se colisiona con un collider que sí es trigger.
    protected virtual void OnTriggerEnter(Collider other)
    {
        // Chocar contra player
        
        // Chocar contra disparo o ataque del player
        
        // Paredes
        
        // Obstáculos
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        if(currentHp <= 0)
            Destroy(gameObject);
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }
}
