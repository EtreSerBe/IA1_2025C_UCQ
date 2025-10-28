using UnityEngine;

public class EnemyBaseState : BaseState
{
    protected EnemyFSM OwnerEnemyFsm;
    protected BaseEnemy EnemyContext;
    protected Coroutine AttackCoroutine;
    
    public override void SetFsm(BaseFSM owner)
    {
        OwnerFsm = owner;
        OwnerEnemyFsm = (EnemyFSM)owner;
        EnemyContext = OwnerEnemyFsm.EnemyContext;
    }
     
    public override void OnEnter()
    {
        base.OnEnter(); // Esto es mandar a llamar la función de la clase padre.
        
        // 1) se acerca al jugador. Implica movimiento. Implica navMeshAgent
        EnemyContext.GoToTargetPlayer();
    }
}
