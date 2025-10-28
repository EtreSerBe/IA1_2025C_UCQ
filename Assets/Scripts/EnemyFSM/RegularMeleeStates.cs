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
               if(AttackCoroutine == null) // Si es null, quiere decir que la corrutina no está activa.
                    AttackCoroutine = StartCoroutine( DoBasicAttack());
          }
     }
     
     private IEnumerator DoBasicAttack()
     {
         // Esperamos el tiempo que se tardaría el ataque en realizarse
         yield return new WaitForSeconds(1.0f);
         
         // ya que se acabó ese tiempo quiere decir que se realizó el ataque,
         // y pasamos al siguiente estado. Por ahora vamos a decir que pasa al ataque de área
         OwnerFsm.ChangeState<AreaMeleeAttackState>();
         AttackCoroutine = null;
         yield break; // yield break para salirnos de la corrutina inmediatamente y cancelarla.
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
               if(AttackCoroutine == null) // Si es null, quiere decir que la corrutina no está activa.
                    AttackCoroutine = StartCoroutine( DoAttack());
          }
     }
     
     private IEnumerator DoAttack()
     {
          // Esperamos el tiempo que se tardaría el ataque en realizarse
          yield return new WaitForSeconds(2.0f);
         
          // ya que se acabó ese tiempo quiere decir que se realizó el ataque,
          // y pasamos al siguiente estado. Por ahora vamos a decir que pasa al ataque de Dash
          OwnerFsm.ChangeState<DashMeleeAttackState>();
          AttackCoroutine = null;
          yield break; // yield break para salirnos de la corrutina inmediatamente y cancelarla.
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
               // Activamos el ataque básico. Solo si no está ejecutándose ya.
               if(AttackCoroutine == null) // Si es null, quiere decir que la corrutina no está activa.
                    AttackCoroutine = StartCoroutine( DoAttack());
          }
     }
     
     private IEnumerator DoAttack()
     {
          // Esperamos el tiempo que se tardaría el ataque en realizarse
          yield return new WaitForSeconds(1.5f);
         
          // ya que se acabó ese tiempo quiere decir que se realizó el ataque,
          // y pasamos al siguiente estado. Por ahora vamos a decir que pasa al ataque de Ultimate
          OwnerFsm.ChangeState<UltimateMeleeAttackState>();
          AttackCoroutine = null;
          yield break; // yield break para salirnos de la corrutina inmediatamente y cancelarla.
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
               if(AttackCoroutine == null) // Si es null, quiere decir que la corrutina no está activa.
                    AttackCoroutine = StartCoroutine( DoAttack());
          }
     }
     
     private IEnumerator DoAttack()
     {
          // Esperamos el tiempo que se tardaría el ataque en realizarse
          yield return new WaitForSeconds(3.5f);
         
          // ya que se acabó ese tiempo quiere decir que se realizó el ataque,
          // y pasamos al siguiente estado. Por ahora vamos a decir que pasa al ataque de Ultimate
          OwnerFsm.ChangeState<BasicMeleeAttackState>();
          AttackCoroutine = null;
          yield break; // yield break para salirnos de la corrutina inmediatamente y cancelarla.
     }
}