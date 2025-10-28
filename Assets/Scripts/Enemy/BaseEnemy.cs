using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

[RequireComponent(typeof(NavMeshAgent), typeof(SensorySystem))]
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
    // protected Senses SensesSystem;
    protected SensorySystem SensesSystem;

    public SensorySystem GetSensesSystem()
    {
        return SensesSystem;
    }

    protected NavMeshAgent NavAgent;

    // Función para que los estados de la FSM le puedan poner una posición objetivo, 
    // sin exponer el NavMeshAgent a cambios.
    public void SetDestination(Vector3 position)
    {
        NavAgent.isStopped = true;
    }
    public void StopMoving()
    {
        NavAgent.destination = transform.position;
    }

    // ME FALTA ASIGNAR EL TARGET PLAYER!
    private Player _targetPlayer;

    public Vector3 GetTargetPlayerPosition()
    {
        if(_targetPlayer)
            return _targetPlayer.transform.position;
        return Vector3.zero;
    }
    public void GoToTargetPlayer()
    {
        if (_targetPlayer == null)
        {
            Debug.Log($"El enemigo {gameObject.name} no tiene asignado un target player en su go to target");
            return; // Nos salimos para evitar que haga null reference exception
        }
        NavAgent.destination = _targetPlayer.transform.position;
    }

    public bool HasDestination()
    {
        return !NavAgent.isStopped;
    }
    
    public enum EDamageMethodType
    {
        DirectClass,
        IDamageableInterface,
        Events
    }

    public EDamageMethodType currentDamageMethodType = EDamageMethodType.DirectClass;

    public float GetAttackRange()
    {
        return attackRange;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        if (NavAgent == null)
        {
            Debug.LogError($"El gameObject {gameObject.name} falló en obtener su navMeshAgent. Favor de verificar");
        }
        SensesSystem = GetComponent<SensorySystem>();
        if (SensesSystem == null)
        {
            Debug.LogError($"El gameObject {gameObject.name} falló en obtener su SensorySystem. Favor de verificar");
        }
    }

    private void FixedUpdate()
    {
        List<GameObject> sensedObjects = SensesSystem.SensedObjects;
        if (sensedObjects != null && sensedObjects.Count > 0) // tiene que ser válido y tener al menos 1 elemento
        {
            _targetPlayer = SensesSystem.SensedObjects[0].GetComponent<Player>();
            if (_targetPlayer == null)
            {
                Debug.LogError($"El gameObject: {SensesSystem.SensedObjects[0].name} no" +
                               $"tiene un script de Player pero fue detectado por el sistema sensorial," +
                               $"favor de verificar");
            }

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
        if (currentHp <= 0)
        {
            Debug.Log($"Destruyendo el gameObject: {gameObject.name}");
            Destroy(gameObject);
        }
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }
}
