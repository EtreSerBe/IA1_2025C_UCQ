using UnityEngine;

public class BasicRangedAttackState : EnemyBaseState
{
     private int _failedShotCounter = 0;
     private int _attackCounter = 0;
     
     void Awake()
     {
          stateName = "Basic Ranged";
     }
     
     public override void OnFixedUpdate()
     {

          EnemyContext.GoToTargetPlayer();
          
          // 2) Cuando está cerca del jugador (dentro del rango del ataque), hace el ataque básico
          Vector3 playerPosition = EnemyContext.GetTargetPlayerPosition();
          if (Utilities.IsObjectInRange(transform.position, playerPosition, EnemyContext.GetAttackRange()))
          {
               // Activamos el ataque básico. Solo si no está ejecutándose ya.
               // if(AttackCoroutine == null) // Si es null, quiere decir que la corrutina no está activa.
               //      AttackCoroutine = StartCoroutine( DoBasicAttack());
               // Si este bool de la animación de basicSlashAttack no está activado, entonces sí lo ponemos como true.
               if(!EnemyContext.enemyAnimator.GetBool(SwordsmanEnemy.BasicRangedAttackHashId))
               {

                    if (_attackCounter > 0)
                    {
                         TriggerAttack();
                         _attackCounter--;
                    }
                    else
                    {
                         Debug.Log("basic ranged está cambiando a area melee");
                         OwnerFsm.ChangeState<AreaMeleeAttackState>();
                    }
               }
          }
     }

     // nuestro proyectil nos diría el resultado del disparo
     // Si falló, incrementamos _failedShotCounter,
     // si _failedShotCounter >= 2,
     //   entonces:           OwnerFsm.ChangeState<AreaRangedAttackState>();
     // 
     
     // Vamos a activar las variables de la Máquina de estados de animación (Animator) para que se triggeree la animación
     // de la acción que queremos.
     private void TriggerAttack()
     {
          Debug.Log($"triggereando el BasicRangedAttack número : {_attackCounter}");

          EnemyContext.enemyAnimator.SetBool(SwordsmanEnemy.BasicRangedAttackHashId, true);
          // la animación que se triggerea con esto es la que va a disparar el proyectil.
          EnemyContext.enemyAnimator.Play(SwordsmanEnemy.BasicRangedAttackHashId);
     }

     public override void OnEnter()
     {
          base.OnEnter();
          _failedShotCounter = 0; // se reinicia a 0.
          // Cada que se entre a este estado, vamos a hacer un random, entre 1 y 3 de cuántas veces se tiene que repetir
          _attackCounter = Random.Range(1, 4); // rango de 1 a 3 (el 4 se excluye) pe
          Debug.Log($"El BasicRangedAttack se va a ejecutar : {_attackCounter} veces");
     }
     
}

public class AreaRangedAttackState : EnemyBaseState
{
     void Awake()
     {
          stateName = "Area Ranged";
     }

     public override void OnFixedUpdate()
     {
          EnemyContext.GoToTargetPlayer();
          
          // 2) Cuando está cerca del jugador (dentro del rango del ataque), hace el ataque básico
          Vector3 playerPosition = EnemyContext.GetTargetPlayerPosition();
          if (Utilities.IsObjectInRange(transform.position, playerPosition, EnemyContext.GetAttackRange()))
          {
               // Activamos el ataque básico. Solo si no está ejecutándose ya.
               // if(AttackCoroutine == null) // Si es null, quiere decir que la corrutina no está activa.
               //      AttackCoroutine = StartCoroutine( DoAttack());
               if(!EnemyContext.enemyAnimator.GetBool(SwordsmanEnemy.AreaRangedAttackHashId))
                    TriggerAttack();
          }
     }
     
     // Vamos a activar las variables de la Máquina de estados de animación (Animator) para que se triggeree la animación
     // de la acción que queremos.
     private void TriggerAttack()
     {
          EnemyContext.enemyAnimator.SetBool(SwordsmanEnemy.AreaRangedAttackHashId, true);
     }

}
