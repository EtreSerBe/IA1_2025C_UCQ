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
    BasicRangedAttack,
    MAX
}

public class SwordsmanEnemy : BaseEnemy
{
    [SerializeField] protected Collider swordCollider;

    public float currentAttackRange = 0.0f;
    public int currentAttackHashId = -1;
    public int currentAttackDamage = 0;

    private bool _isAttackReady = false;
    public bool IsAttackReady => _isAttackReady;

    public float parabolicJumpHeight = 2.0f;
    public float parabolicJumpDuration = 0.5f;
    
    // El estado le pide al contexto cuál ataque va a ejecutar ahora
     
    public void ChangeAttack(ESwordsmanAttacks newAttack)
    {
        switch (newAttack)
        {
            case ESwordsmanAttacks.BasicSlashAttack:
                currentAttackRange = 1.0f;
                currentAttackDamage = 1;
                EnemyFsm.ChangeState<BasicMeleeAttackState>();
                break;
            case ESwordsmanAttacks.AreaSlashAttack:
                currentAttackRange = 1.5f;
                currentAttackDamage = 3;
                EnemyFsm.ChangeState<AreaMeleeAttackState>();
                break;
            case ESwordsmanAttacks.DashSlashAttack:
                currentAttackRange = 5.0f;
                currentAttackDamage = 2;
                EnemyFsm.ChangeState<DashMeleeAttackState>();
                break;
            case ESwordsmanAttacks.UltimateSlashAttack:
                currentAttackRange = 3.0f;
                currentAttackDamage = 10;
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
        NavAgent.isStopped = false; 
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
                break;
            case ESwordsmanAttacks.AreaSlashAttack:
                // enemyAnimator.SetBool("AreaSlashAttack", false);
                enemyAnimator.SetBool(AreaSlashAttackHashId, false);
                break;
            case ESwordsmanAttacks.DashSlashAttack:
                enemyAnimator.SetBool(DashSlashAttackHashId, false);
                break;
            case ESwordsmanAttacks.UltimateSlashAttack:
                enemyAnimator.SetBool(UltimateSlashAttackHashId, false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(attackID), attackID, null);
        }
        
        // Después elegimos el ataque que sigue.
        SelectNextState();
    }

    private void SelectNextState()
    {

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
            ChangeAttack(ESwordsmanAttacks.BasicSlashAttack);
            return;
        }
        
        // Elección de acción melee random
        // ESwordsmanAttacks newAttack = (ESwordsmanAttacks)Random.Range((int)ESwordsmanAttacks.BasicSlashAttack, (int)ESwordsmanAttacks.MAX);
        // ChangeAttack(newAttack);
    }

}
