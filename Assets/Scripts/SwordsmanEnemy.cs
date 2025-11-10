using System;
using UnityEngine;
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

    public void Start()
    {
        // Para que empiece con los datos del basic attack.
        ChangeAttack(ESwordsmanAttacks.BasicSlashAttack);
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
        
        // // Después elegimos el ataque que sigue.
        // // Si es la primera vez en el estado de ataque básico O el estado anterior fue Ranged Ultimate, 
        // if(EnemyFsm.CurrentStateType == typeof(BasicMeleeAttackState))
        // {
        //     if (EnemyFsm.PreviousStates.Length < 2 || 
        //         EnemyFsm.PreviousStates[^2] == typeof(RangedState))
        //     {
        //         // Entonces es Dash o Area con un 50/50
        //         if (Random.value < 0.5f)
        //         {
        //             ChangeAttack(ESwordsmanAttacks.DashSlashAttack);
        //             return;
        //         }
        //         else
        //         {
        //             ChangeAttack(ESwordsmanAttacks.AreaSlashAttack);
        //             return;
        //         }
        //     }
        // }
        // // Manejo del Ataque Area
        // if (EnemyFsm.CurrentStateType == typeof(AreaMeleeAttackState))
        // {
        //     // Si hace 2 acciones hiciste el ataque Dash, entonces vete a ultimate; si no, vete a basic
        //     if (EnemyFsm.PreviousStates[^3] == typeof(DashMeleeAttackState))
        //     {
        //         ChangeAttack(ESwordsmanAttacks.UltimateSlashAttack);
        //         return;
        //     }
        //     else
        //     {
        //         ChangeAttack(ESwordsmanAttacks.BasicSlashAttack);
        //         return;
        //     }
        // }
        //
        // // Manejo del Ataque Dash
        // if (EnemyFsm.CurrentStateType == typeof(DashMeleeAttackState))
        // {
        //     if (EnemyFsm.PreviousStates[^3] == typeof(AreaMeleeAttackState))
        //     {
        //         ChangeAttack(ESwordsmanAttacks.UltimateSlashAttack);
        //         return;
        //     }
        //     else
        //     {
        //         ChangeAttack(ESwordsmanAttacks.BasicSlashAttack);
        //         return;
        //     }
        // }
        //
        // // Manejo del Ataque Ultimate.
        // // Ya que se acabó el ultimate, pásate al estado ranged Basic.
        // if (EnemyFsm.CurrentStateType == typeof(UltimateMeleeAttackState))
        // {
        //     ChangeAttack(ESwordsmanAttacks.BasicRangedAttack);
        //     return;
        // }
        
        // Elección de acción melee random
        ESwordsmanAttacks newAttack = (ESwordsmanAttacks)Random.Range((int)ESwordsmanAttacks.BasicSlashAttack, (int)ESwordsmanAttacks.MAX);
        ChangeAttack(newAttack);
    }
    

}
