using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public enum ESwordsmanAttacks
{
    None,
    BasicSlashAttack,
    AreaSlashAttack,
    DashSlashAttack,
    UltimateSlashAttack,
    UltimateStaggered,
    BasicRangedAttack,
    AreaRangedAttack,
    DashRangedAttack,
    UltimateRangedAttack,
    MAX
}

public class SwordsmanEnemy : BaseEnemy
{
    [SerializeField] protected Collider swordCollider;
    [SerializeField] protected Projectile basicRangedAttackProjectilePrefab;
    
    public float currentAttackRange = 0.0f;
    public int currentAttackHashId = -1;
    public int currentAttackDamage = 0;
    public float currentAttackCooldown = 0;

    private bool _isAttackReady = false;
    public bool IsAttackReady => _isAttackReady;

    public float parabolicJumpHeight = 2.0f;
    public float parabolicJumpDuration = 0.5f;

    [SerializeField] protected SO_AttackParameters basicSlashAttackParameters; 
    [SerializeField] protected SO_AttackParameters areaSlashAttackParameters;
    [SerializeField] protected SO_AttackParameters dashSlashAttackParameters;
    [SerializeField] protected SO_AttackParameters ultimateSlashAttackParameters;

    private float _basicSlashRemainingCooldown = 0;
    private float _areaSlashRemainingCooldown = 0;
    private float _dashSlashRemainingCooldown = 0;
    private float _ultimateSlashRemainingCooldown = 0;

    public float rangedStatePreferredCombatDistance = 5.0f;
    
    // El estado le pide al contexto cuál ataque va a ejecutar ahora
     
    public void ChangeAttack(ESwordsmanAttacks newAttack)
    {
        switch (newAttack)
        {
            case ESwordsmanAttacks.BasicSlashAttack:
                currentAttackRange = basicSlashAttackParameters.Range;
                currentAttackDamage = basicSlashAttackParameters.Damage;
                currentAttackCooldown = basicSlashAttackParameters.Cooldown;
                EnemyFsm.ChangeState<BasicMeleeAttackState>();
                break;
            case ESwordsmanAttacks.AreaSlashAttack:
                currentAttackRange = areaSlashAttackParameters.Range;
                currentAttackDamage = areaSlashAttackParameters.Damage;
                currentAttackCooldown = areaSlashAttackParameters.Cooldown;
                EnemyFsm.ChangeState<AreaMeleeAttackState>();
                break;
            case ESwordsmanAttacks.DashSlashAttack:
                currentAttackRange = dashSlashAttackParameters.Range;
                currentAttackDamage = dashSlashAttackParameters.Damage;
                EnemyFsm.ChangeState<DashMeleeAttackState>();
                break;
            case ESwordsmanAttacks.UltimateSlashAttack:
                if (_ultimateSlashRemainingCooldown > 0.0f)
                {
                    Debug.Log($"Se hizo el Basic Slash en vez del últimate slash porque está en cooldown.");
                    currentAttackRange = basicSlashAttackParameters.Range;
                    currentAttackDamage = basicSlashAttackParameters.Damage;
                    // entonces el ultimate está en cooldown y no se puede usar. Hacer otra acción distinta.
                    EnemyFsm.ChangeState<BasicMeleeAttackState>();
                    return;
                }
                // si no está en cooldown, pasamos al estado del ultimate
                currentAttackRange = ultimateSlashAttackParameters.Range;
                currentAttackDamage = ultimateSlashAttackParameters.Damage;
                EnemyFsm.ChangeState<UltimateMeleeAttackState>();
                break;
            case ESwordsmanAttacks.BasicRangedAttack:
                currentAttackRange = 10.0f;
                currentAttackDamage = 1;
                EnemyFsm.ChangeState<RangedState>();
                break;
            case ESwordsmanAttacks.None:
            default:
                throw new ArgumentOutOfRangeException(nameof(newAttack), newAttack, null);
        }
    }
        
    // Necesitamos un int para cada parámetro que nos interese del animator
    public static readonly int BasicSlashAttackHashId = Animator.StringToHash("BasicSlashAttack");
    public static readonly int AreaSlashAttackHashId = Animator.StringToHash("AreaSlashAttack");
    public static readonly int DashSlashAttackHashId = Animator.StringToHash("DashSlashAttack");
    public static readonly int UltimateSlashAttackHashId = Animator.StringToHash("UltimateSlashAttack");
    public static readonly int UltimateSlashStaggeredHashId = Animator.StringToHash("UltimateSlashStaggered");

    
    public static readonly int BasicRangedAttackHashId = Animator.StringToHash("BasicRangedAttack");
    public static readonly int AreaRangedAttackHashId = Animator.StringToHash("AreaRangedAttack");
    public static readonly int DashRangedAttackHashId = Animator.StringToHash("DashRangedAttack");
    public static readonly int UltimateRangedAttackHashId = Animator.StringToHash("UltimateRangedAttack");
    
    public static readonly int JumpHashId = Animator.StringToHash("Jump");


    // Hashes para Estados del animator
    public static readonly int JumpingDownStateHashId = Animator.StringToHash("Jumping Down");

    
    
    private bool _isInJumpingSequence = false;
    
    
    
    // Corrutina que siempre está checando si el agente está sobre un nav mesh link.
    
    
    
    public void Start()
    {
        // Para que empiece con los datos del basic attack.
        ChangeAttack(ESwordsmanAttacks.BasicSlashAttack);
        StartCoroutine(CheckForOffMeshLink());
    }

    public void Update()
    {
        if (NavAgent.isOnOffMeshLink == false && _isInJumpingSequence)
        {
            _isInJumpingSequence = false; // ahora ya se podría iniciar otro brinco distinto.
        }
    }

    // Qué tanta distancia respecto al player tiene que estar
    void GoToPositionRelativeToPlayer(float distanceToPlayer)
    {
        if (TargetPlayer == null)
        {
            Debug.Log($"El enemigo {gameObject.name} no tiene asignado un target player en su go to target");
            return; // Nos salimos para evitar que haga null reference exception
        }
        // en vez de irnos directamente a su posición, nos vamos a 
        Vector3 directionFromPlayerToThis = transform.position - TargetPlayer.transform.position; 
        NavAgent.destination = TargetPlayer.transform.position + directionFromPlayerToThis.normalized * distanceToPlayer;
    }
    


    // Función usada por los Animation Events para que el personaje se mueva o no.
    void SetCanMove(int canMove)
    {
        NavAgent.isStopped = canMove == 0;
    }

    void SetIsKinematic(int isKinematic)
    {
        rb.isKinematic = isKinematic == 1;
    }
    
    IEnumerator CheckForOffMeshLink()
    {
        while (true)
        {
            // Si está sobre un off mesh link Y no está brincando actualmente...
            if (NavAgent.isOnOffMeshLink && !_isInJumpingSequence)
            {
                Debug.Log($"El personaje {gameObject.name} empezó de brincar con su Off mesh Link");
                NavAgent.isStopped = true; // le digo al agente que NO se puede seguir moviendo sino hasta que
                                           // llegue el animation event de que el brinco ya dio el impulso.
                enemyAnimator.SetBool(JumpHashId, true);
                _isInJumpingSequence = true; // esta variable es la que determina que actualmente está brincando.

                enemyAnimator.Play(JumpingDownStateHashId);
                // yield return StartCoroutine(ParabolicJump(NavAgent, parabolicJumpHeight, parabolicJumpDuration));
                // NavAgent.CompleteOffMeshLink();
            }

            yield return null;
        }
    }
    
    public void BeginJumping()
    {
        // El agente puede comenzar a moverse con el impulso de ese brinco.
        StartCoroutine(ParabolicJump(NavAgent, parabolicJumpHeight, parabolicJumpDuration));
    }
    
    IEnumerator ParabolicJump(NavMeshAgent navAgent, float height, float duration)
    {
        var offMeshLinkData = NavAgent.currentOffMeshLinkData;
        Vector3 startPosition = navAgent.transform.position; // offMeshLinkData.startPos;
        Vector3 endPosition = offMeshLinkData.endPos + Vector3.up * navAgent.baseOffset;
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {
            float yOffset = height * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
            navAgent.transform.position =
                Vector3.Lerp(startPosition, endPosition, normalizedTime) + yOffset * Vector3.up;
                
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        
        // Cuando se sale del while de arriba, quiere decir que ya se terminó el desplazamiento del brinco. 
        // Esto hace que deje de hacer la animación de Caída y pase a la de Aterrizaje, 
        // que cuando avance más, activa el evento de animación que manda a llamar FinishedJumpLanding().
        enemyAnimator.SetBool(JumpHashId, false);

    }
    
    public void FinishedJumpLanding()
    {
        Debug.Log($"El personaje {gameObject.name} terminó de brincar y aterrizar con su Off mesh Link");

        
        // Le decimos que ya llegó a la parte de la animación en la que ya no está detenido y puede seguir moviéndose.
        // NavAgent.isStopped = false; 
        NavAgent.CompleteOffMeshLink();
    }
    
    // Esta función se va a mandar a llamar por el animation event y nos dice qué acción del 
    // swordsman está realizando
    void BeginAttackActiveFrames(ESwordsmanAttacks attackID)
    {
        // Envía que sucedió este event
        Debug.Log($"Empezó los frames activos del ataque: {attackID}");
        // el collider de la espada se activa
        swordCollider.gameObject.SetActive(true);
    }
    
    void EndAttackActiveFrames(ESwordsmanAttacks attackID)
    {
        // Envía que sucedió este event
        Debug.Log($"Terminaron los frames activos del ataque: {attackID}");
        // el collider de la espada se desactiva
        swordCollider.gameObject.SetActive(false);
    }
    
    void AttackPreparationReady(ESwordsmanAttacks attackID)
    {
        enemyAnimator.SetBool("AttackIsReady", true);
        Debug.Log($"La animación del ataque: {attackID} indica que ya se puede realizar la acción real del personaje");
        // switch (attackID)
        // {
        //     case ESwordsmanAttacks.None:
        //         break;
        //     case ESwordsmanAttacks.BasicSlashAttack:
        //         // enemyAnimator.SetBool("BasicSlashAttack", false); // versión lenta
        //         enemyAnimator.SetBool(BasicSlashAttackHashId, false); // Versión optimizada
        //         // EnemyFsm.ChangeState<AreaMeleeAttackState>();
        //         break;
        //     case ESwordsmanAttacks.AreaSlashAttack:
        //         // enemyAnimator.SetBool("AreaSlashAttack", false);
        //         enemyAnimator.SetBool(AreaSlashAttackHashId, false);
        //         break;
        //     case ESwordsmanAttacks.DashSlashAttack:
        //         enemyAnimator.SetBool(DashSlashAttackHashId, false);
        //         break;
        //     case ESwordsmanAttacks.UltimateSlashAttack:
        //         enemyAnimator.SetBool(UltimateSlashAttackHashId, false);
        //         break;
        //     default:
        //         throw new ArgumentOutOfRangeException(nameof(attackID), attackID, null);
        // }
        //
        // // Después elegimos el ataque que sigue.
        // SelectNextState();
    }

    void BasicRangedAttack()
    {
        // spawnea un objeto de la clase bala/proyectil que tenga X comportamiento.
        Projectile basicRangedAttackProjectile = Instantiate(basicRangedAttackProjectilePrefab, transform.position, transform.rotation);
        basicRangedAttackProjectile.Shoot();
    }
    
    void FinishedAttack(ESwordsmanAttacks attackID)
    {
        Debug.Log($"Terminó la animación del ataque: {attackID}");
        switch (attackID)
        {
            case ESwordsmanAttacks.None:
                break;
            case ESwordsmanAttacks.BasicSlashAttack:
                // enemyAnimator.SetBool("BasicSlashAttack", false); // versión lenta
                enemyAnimator.SetBool(BasicSlashAttackHashId, false); // Versión optimizada
                // EnemyFsm.ChangeState<AreaMeleeAttackState>();
                _basicSlashRemainingCooldown = basicSlashAttackParameters.Cooldown;
                break;
            case ESwordsmanAttacks.AreaSlashAttack:
                // enemyAnimator.SetBool("AreaSlashAttack", false);
                enemyAnimator.SetBool(AreaSlashAttackHashId, false);
                _areaSlashRemainingCooldown = areaSlashAttackParameters.Cooldown;
                break;
            case ESwordsmanAttacks.DashSlashAttack:
                enemyAnimator.SetBool(DashSlashAttackHashId, false);
                _dashSlashRemainingCooldown = dashSlashAttackParameters.Cooldown;
                break;
            case ESwordsmanAttacks.UltimateSlashAttack:
                enemyAnimator.SetBool(UltimateSlashAttackHashId, false);
                // Le ponemos el cooldown de dicho ataque.
                _ultimateSlashRemainingCooldown = ultimateSlashAttackParameters.Cooldown;
                break;
            case ESwordsmanAttacks.UltimateStaggered:
                enemyAnimator.SetBool(UltimateSlashStaggeredHashId, false);
                break;
            case ESwordsmanAttacks.BasicRangedAttack:
                enemyAnimator.SetBool(BasicRangedAttackHashId, false);
                return; // NOTA: aquí es return para que no se vaya al select next state.
            default:
                throw new ArgumentOutOfRangeException(nameof(attackID), attackID, null);
        }
        
        // Después elegimos el ataque que sigue.
        SelectNextState();
    }

    private void SelectNextState()
    {
        Debug.Log("Entrando al Select Next State");
        // EnemyFsm.ChangeState<BasicRangedAttackState>();
        
        // Si es la primera vez en el estado de ataque básico O el estado anterior fue Ranged Ultimate, 
        if(EnemyFsm.CurrentStateType == typeof(BasicMeleeAttackState))
        {
            if (EnemyFsm.StatesHistory.Length < 2 || 
                EnemyFsm.StatesHistory[^2] == typeof(UltimateMeleeAttackState))
            {
                // Entonces es Dash o Area con un 50/50
                if (Random.value < 0.5f)
                {
                    ChangeAttack(ESwordsmanAttacks.DashSlashAttack);
                    return;
                }

                ChangeAttack(ESwordsmanAttacks.AreaSlashAttack);
                return;
            }
            if (EnemyFsm.StatesHistory.Length >= 2) // Si tiene al menos otra acción antes de este basic attack
            {
                // checamos si fue un area slash
                if (EnemyFsm.StatesHistory[^2] == typeof(AreaMeleeAttackState))
                {
                    // entonces toca hacer el dash
                    ChangeAttack(ESwordsmanAttacks.DashSlashAttack);
                    return;
                }
                // checamos si fue un dash slash
                if (EnemyFsm.StatesHistory[^2] == typeof(DashMeleeAttackState))
                {
                    // entonces toca hacer el de area
                    ChangeAttack(ESwordsmanAttacks.AreaSlashAttack);
                    return;
                }
            }
        }
        
        // Manejo del Ataque Area
        if (EnemyFsm.CurrentStateType == typeof(AreaMeleeAttackState))
        {
            // Si hace 2 acciones hiciste el ataque Dash, entonces vete a ultimate; si no, vete a basic
            if (EnemyFsm.StatesHistory.Length >= 3 && 
                EnemyFsm.StatesHistory[^3] == typeof(DashMeleeAttackState))
            {
                ChangeAttack(ESwordsmanAttacks.UltimateSlashAttack);
                return;
            }
            ChangeAttack(ESwordsmanAttacks.BasicSlashAttack);
            return;
        }
        
        // Manejo del Ataque Dash
        if (EnemyFsm.CurrentStateType == typeof(DashMeleeAttackState))
        {
            // si hace 2 acciones hizo el ataque de área, entonces toca hacer el ultimate
            if (EnemyFsm.StatesHistory.Length >= 3 && 
              EnemyFsm.StatesHistory[^3] == typeof(AreaMeleeAttackState))
            {
                ChangeAttack(ESwordsmanAttacks.UltimateSlashAttack);
                return;
            }

            ChangeAttack(ESwordsmanAttacks.BasicSlashAttack);
            return;
        }
        
        // Manejo del Ataque Ultimate.
        // Ya que se acabó el ultimate, pásate al estado ranged Basic.
        if (EnemyFsm.CurrentStateType == typeof(UltimateMeleeAttackState))
        {
            EnemyFsm.ChangeState<BasicRangedAttackState>();
            // ChangeAttack(ESwordsmanAttacks.BasicSlashAttack);
            return;
        }
        
        // Elección de acción melee random
        // ESwordsmanAttacks newAttack = (ESwordsmanAttacks)Random.Range((int)ESwordsmanAttacks.BasicSlashAttack, (int)ESwordsmanAttacks.MAX);
        // ChangeAttack(newAttack);
    }
    
    
    // public void TakeDamage(int damage)
    // {
    //     currentHp -= damage;
    //     if (currentHp <= 0)
    //     {
    //         Debug.Log($"Destruyendo el gameObject: {gameObject.name}");
    //         Destroy(gameObject);
    //     }
    // }


}
