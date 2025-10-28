// using System;
// using System.Collections;
// using UnityEngine;
//
//
// public enum EMeleeStateActions
// {
//     BasicAttack,
//     DashAttack,
//     AreaAttack,
//     UltimateAttack
// }
//
//
// /*
//  *  // El sub estado de basic attack:
//    // 1) se acerca al jugador. Implica movimiento. Implica navMeshAgent
//    // 2) Cuando está cerca del jugador, hace el ataque básico
//    // 3) termina el ataque básico.
//    // 4) se va al ataque dash o al ataque de área
//  * 
//  */
//
// public class MeleeState : BaseState
// {
//     // El sub-estado actual del estado melee.
//     private EMeleeStateActions _currentState;
//     EnemyFSM ownerEnemyFsm;
//     private BaseEnemy enemyContext;
//     
//     public override void OnEnter()
//     {
//         // El subestado inicial de este estado es el basic attack.
//         _currentState = EMeleeStateActions.BasicAttack;
//     }
//
//     private void OnEnterSubstate()
//     {
//         switch (_currentState)
//         {
//             case EMeleeStateActions.BasicAttack:
//                 // El sub estado de basic attack:
//                 // 1) se acerca al jugador. Implica movimiento. Implica navMeshAgent
//                 enemyContext.GoToTargetPlayer();
//                 // 2) hace el ataque básico
//                 // 3) termina el ataque básico.
//                 // 4) se va al ataque dash o al ataque de área
//                 break;
//             case EMeleeStateActions.DashAttack:
//                 // 1) voltea hacia donde está el jugador
//                 // 2) hace el dash
//                 // 3) termina la animación del dash
//                 // 4) animación de cooldown del dash
//                 break;
//             case EMeleeStateActions.AreaAttack:
//                 break;
//             case EMeleeStateActions.UltimateAttack:
//                 break;
//             default:
//                 throw new ArgumentOutOfRangeException();
//         }
//     }
//
//     private IEnumerator DoBasicAttack()
//     {
//         // Esperamos el tiempo que se tardaría el ataque en realizarse
//         yield return new WaitForSeconds(1.0f);
//         
//         // ya que se acabó ese tiempo quiere decir que se realizó el ataque, y pasamos al siguiente estado
//         OwnerFsm.ChangeState<>();
//     }
//
//     public override void OnUpdate()
//     {
//         switch (_currentState)
//         {
//             case EMeleeStateActions.BasicAttack:
//                 // 2) Cuando está cerca del jugador (dentro del rango del ataque), hace el ataque básico
//                 Vector3 playerPosition = enemyContext.GetTargetPlayerPosition;
//                 Utilities.IsObjectInRange(transform.position, playerPosition, enemyContext.GetAttackRange());
//                 // Activamos el ataque básico.
//                 
//                 // 3) termina el ataque básico.
//                 // 4) se va al ataque dash o al ataque de área
//                 break;
//             case EMeleeStateActions.DashAttack:
//                 // 1) voltea hacia donde está el jugador
//                 // 2) hace el dash
//                 // 3) termina la animación del dash
//                 // 4) animación de cooldown del dash
//                 break;
//             case EMeleeStateActions.AreaAttack:
//                 break;
//             case EMeleeStateActions.UltimateAttack:
//                 break;
//             default:
//                 throw new ArgumentOutOfRangeException();
//         }
//     }
//
//     void Awake()
//     {
//         stateName = "Melee";
//         ownerEnemyFsm = (EnemyFSM)OwnerFsm;
//         enemyContext = ownerEnemyFsm.EnemyContext;
//     }
// }
