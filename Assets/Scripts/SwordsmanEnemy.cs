using System;
using UnityEngine;


public enum ESwordsmanAttacks
{
    None,
    BasicSlashAttack,
    AreaSlashAttack,
}

public class SwordsmanEnemy : BaseEnemy
{
    [SerializeField] protected Collider swordCollider;

        
    // Necesitamos un int para cada parámetro que nos interese del animator
    public static readonly int BasicSlashAttackHashId = Animator.StringToHash("BasicSlashAttack");
    public static readonly int AreaSlashAttackHashId = Animator.StringToHash("AreaSlashAttack");
    public static readonly int DashSlashAttackHashId = Animator.StringToHash("DashSlashAttack");
    public static readonly int UltimateSlashAttackHashId = Animator.StringToHash("UltimateSlashAttack");

    
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
                EnemyFsm.ChangeState<AreaMeleeAttackState>();
                break;
            case ESwordsmanAttacks.AreaSlashAttack:
                enemyAnimator.SetBool("AreaSlashAttack", false);
                enemyAnimator.SetBool("AreaSlashAttack", false);
                EnemyFsm.ChangeState<BasicMeleeAttackState>();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(attackID), attackID, null);
        }
    }
    

}
