using System;
using System.Collections;
using UnityEngine;



public class BasicMeleeAttackState : EnemyBaseState
{
     void Awake()
     {
          stateName = "Basic Melee";
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
               if(!EnemyContext.enemyAnimator.GetBool(SwordsmanEnemy.BasicSlashAttackHashId))
                    TriggerAttack();
          }
     }

     // Vamos a activar las variables de la Máquina de estados de animación (Animator) para que se triggeree la animación
     // de la acción que queremos.
     private void TriggerAttack()
     {
          EnemyContext.enemyAnimator.SetBool(SwordsmanEnemy.BasicSlashAttackHashId, true);
     }
     
}

public class AreaMeleeAttackState : EnemyBaseState
{
     void Awake()
     {
          stateName = "Area Melee";
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
               if(!EnemyContext.enemyAnimator.GetBool(SwordsmanEnemy.AreaSlashAttackHashId))
                    TriggerAttack();
          }
     }
     
     // Vamos a activar las variables de la Máquina de estados de animación (Animator) para que se triggeree la animación
     // de la acción que queremos.
     private void TriggerAttack()
     {
          EnemyContext.enemyAnimator.SetBool(SwordsmanEnemy.AreaSlashAttackHashId, true);
     }

}

public class DashMeleeAttackState : EnemyBaseState
{
     void Awake()
     {
          stateName = "Dash Melee";
     }
     
     public override void OnFixedUpdate()
     {
          EnemyContext.GoToTargetPlayer();
          
          // 2) Cuando está cerca del jugador (dentro del rango del ataque), hace el ataque básico
          Vector3 playerPosition = EnemyContext.GetTargetPlayerPosition();
          if (Utilities.IsObjectInRange(transform.position, playerPosition, EnemyContext.GetAttackRange()))
          {
               if(!EnemyContext.enemyAnimator.GetBool(SwordsmanEnemy.DashSlashAttackHashId))
                    TriggerAttack();
          }
     }
     
     // Vamos a activar las variables de la Máquina de estados de animación (Animator) para que se triggeree la animación
     // de la acción que queremos.
     private void TriggerAttack()
     {
          EnemyContext.enemyAnimator.SetBool(SwordsmanEnemy.DashSlashAttackHashId, true);
     }
}


public class UltimateMeleeAttackState : EnemyBaseState
{
     void Awake()
     {
          stateName = "Ultimate Melee";
     }
     
     public override void OnFixedUpdate()
     {
          EnemyContext.GoToTargetPlayer();
          
          // 2) Cuando está cerca del jugador (dentro del rango del ataque), hace el ataque básico
          Vector3 playerPosition = EnemyContext.GetTargetPlayerPosition();
          if (Utilities.IsObjectInRange(transform.position, playerPosition, EnemyContext.GetAttackRange()))
          {
               // Activamos el ataque básico. Solo si no está ejecutándose ya.
               if(!EnemyContext.enemyAnimator.GetBool(SwordsmanEnemy.UltimateSlashAttackHashId))
                    TriggerAttack();
          }
     }
     
     // Vamos a activar las variables de la Máquina de estados de animación (Animator) para que se triggeree la animación
     // de la acción que queremos.
     private void TriggerAttack()
     {
          EnemyContext.enemyAnimator.SetBool(SwordsmanEnemy.UltimateSlashAttackHashId, true);
     }
     
}




/*
  EJEMPLO DE CÓMO PODRÍAMOS REFACTORIZAR ESTO PARA UN ENEMIGO MELEE MÁS SENCILLO.
public class MeleeAttackState : EnemyBaseState
{
     // El estado le pide al contexto cuál ataque va a ejecutar ahora

     
     void Awake()
     {
          stateName = "Melee Attack";
     }
     public override void OnFixedUpdate()
     {

          EnemyContext.GoToTargetPlayer();
          
          // 2) Cuando está cerca del jugador (dentro del rango del ataque), hace el ataque básico
          Vector3 playerPosition = EnemyContext.GetTargetPlayerPosition();
          float currentAttackRange = ((SwordsmanEnemy)EnemyContext).currentAttackRange;
          if (Utilities.IsObjectInRange(transform.position, playerPosition, currentAttackRange))
          {
               int currentAttackHashId = ((SwordsmanEnemy)EnemyContext).currentAttackHashId;
               // Activamos el ataque básico. Solo si no está ejecutándose ya.
               // if(AttackCoroutine == null) // Si es null, quiere decir que la corrutina no está activa.
               //      AttackCoroutine = StartCoroutine( DoBasicAttack());
               // Si este bool de la animación de basicSlashAttack no está activado, entonces sí lo ponemos como true.
               if(!EnemyContext.enemyAnimator.GetBool(currentAttackHashId))
                    TriggerAttack(currentAttackHashId);
          }
     }

     // Vamos a activar las variables de la Máquina de estados de animación (Animator) para que se triggeree la animación
     // de la acción que queremos.
     private void TriggerAttack(int currentAttackHashId)
     {
          EnemyContext.enemyAnimator.SetBool(currentAttackHashId, true);
     }
}*/