using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;


public class BasicMeleeAttackState : EnemyBaseState
{
     private float _timeToAttackTranscurredTime = 0;
     // Tiempo total (no continuo) que debe transcurrir antes de realizar el ataque
     private float _timeBeforeAttacking;
     private float _maxTimeBeforeAttacking = 2.5f;
     private float _minTimeBeforeAttacking = 0.2f;

     private Coroutine timeToRefreshTranscurredTimeCoroutine;

     private float timeToRefreshTranscurredTimeToAttack = 1.0f;
     
     public override void OnEnter()
     {
          _timeToAttackTranscurredTime = 0;
          _timeBeforeAttacking = Random.Range(_minTimeBeforeAttacking, _maxTimeBeforeAttacking);
     }
     
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
               if (EnemyContext.enemyAnimator.GetBool(SwordsmanEnemy.BasicSlashAttackHashId))
                    return; // nada que hacer, porque ya está realizando este ataque.
               
               // Acumulas tiempo para ver si ya es momento de atacar
               _timeToAttackTranscurredTime += Time.fixedDeltaTime;
               
               // Si ya es momento, entonces...
               if (_timeBeforeAttacking < _timeToAttackTranscurredTime)
               {
                    Debug.Log($"Ya se cumplió el tiempo de {_timeBeforeAttacking} porque " +
                              $"pasaron {_timeToAttackTranscurredTime} de tiempo");
                    _timeToAttackTranscurredTime = 0; // por pura seguridad
                    // Activamos el ataque básico. Solo si no está ejecutándose ya.
                    // Si este bool de la animación de basicSlashAttack no está activado, entonces sí lo ponemos como true.
                    TriggerAttack();
               }
          }
          else // si no está en rango de ataque
          {
               if (_timeToAttackTranscurredTime <= 0) // que no inicie la corrutina si no ha tenido al player cerca.
                    return;
                    
               // decimos que empiece a bajar el contador de tiempo si después de 1 segundo no lo hemos tenido de nuevo en rango
               if (timeToRefreshTranscurredTimeCoroutine != null)
                    return; // si no es null, ya está activa, entonces no hay nada que hacerle.
               
               timeToRefreshTranscurredTimeCoroutine = StartCoroutine(refreshTranscurredTimeToAttack());
          }
     }

     // OJO: este tipo de cosa puede llevar a exploits!
     private IEnumerator refreshTranscurredTimeToAttack()
     {
          float timeCounter = 0;
          while (timeCounter < timeToRefreshTranscurredTimeToAttack)
          {
               timeCounter += Time.deltaTime;
               yield return null;
          }
          
          Debug.Log($"El melee básico se reseteó su tiempo porque el player estuvo fuera de rango más de un segundo");

          _timeToAttackTranscurredTime = 0.0f;
          _timeBeforeAttacking = Random.Range(_minTimeBeforeAttacking, _maxTimeBeforeAttacking);
          timeToRefreshTranscurredTimeCoroutine = null;
     }

     // Vamos a activar las variables de la Máquina de estados de animación (Animator) para que se triggeree la animación
     // de la acción que queremos.
     private void TriggerAttack()
     {
          EnemyContext.enemyAnimator.SetBool(SwordsmanEnemy.BasicSlashAttackHashId, true);
          EnemyContext.enemyAnimator.Play(SwordsmanEnemy.BasicSlashAttackHashId);
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
          EnemyContext.enemyAnimator.Play(SwordsmanEnemy.AreaSlashAttackHashId);
     }

}

public class DashMeleeAttackState : EnemyBaseState
{
     // variable que nos representa que actualmente está apuntando 
     private bool _performingAttack = false;
     private Coroutine _dashCoroutine;
     private Coroutine _rotateTowardsPlayer;
     
     void Awake()
     {
          stateName = "Dash Melee";
     }

     public override void OnEnter()
     {
          base.OnEnter();
          _performingAttack = false;
          if(_dashCoroutine != null)
          {
               StopCoroutine(_dashCoroutine); // Por seguridad nada más. 
               _dashCoroutine = null;
          }
          if(_rotateTowardsPlayer != null)
          {
               StopCoroutine(_rotateTowardsPlayer); // Por seguridad nada más. 
               _rotateTowardsPlayer = null;
          }
     }
     
     public override void OnFixedUpdate()
     {
          // si no está apuntando o haciendo el ataque en sí, sí nos podemos mover
          if(!_performingAttack)
               EnemyContext.GoToTargetPlayer(); // entonces sí nos podemos mover
          
          // Si el ataque ya está listo, se ejecuta la secuencia de hacer el dash tal cual
          if (EnemyContext.enemyAnimator.GetBool("AttackIsReady"))
          {
               // Cuando el ataque esté listo, dejamos de "apuntar"
               if(_rotateTowardsPlayer != null)
               {
                    StopCoroutine(_rotateTowardsPlayer); // Por seguridad nada más. 
                    _rotateTowardsPlayer = null;
               }
               
               // Parte del ataque en sí, lleva un movimiento que no es tal cual controlado por el nav mesh,
               // CREO que sería desactivar el nav mesh agent un instante mientras se hace el ataque,
               // desplazar a este enemigo X distancia en la dirección que se había quedado al apuntar.
               // EnemyContext. // NOTA: Si se necesita desactivarle el navMeshAgent lo haríamos aquí.
               
               // Como este desplazamiento es a través de X segundos -no instantaneamente- usamos una corrutina
               if(_dashCoroutine != null)
               {
                    StopCoroutine(_dashCoroutine); // Por seguridad nada más. 
                    _dashCoroutine = null;
               }
               
               
               _dashCoroutine = StartCoroutine(DashAttackCoroutine());
               // Y quitamos el booleano del animator para que no vuelva a entrar a este if. Que solo entre una vez.
               EnemyContext.enemyAnimator.SetBool("AttackIsReady", false);
          }
          
          // 2) Cuando está cerca del jugador (dentro del rango del ataque), hace el ataque dash
          Vector3 playerPosition = EnemyContext.GetTargetPlayerPosition();
          if (Utilities.IsObjectInRange(transform.position, playerPosition, EnemyContext.GetAttackRange()))
          {
               // Como ya está en rango de ataque, comienza a realizar el ataque
               _performingAttack = true; // hace que deje de actualizarse la ruta después
               EnemyContext.RemoveNavMeshAgentPath(); // hace que deje de moverse hacia donde iba ahorita.
               
               // Empezamos la corrutina de rotar hacia el jugador
               
               // Si ya está en el rango de nuestro dash, quiero que "apunte" su dash hacia el player 
               // durante X tiempo, y después realice el dash hacia la posición + la distancia del dash en esa 
               // dirección
               
               // durante el tiempo de apuntado ya no se va a mover, pero sí va a rotar hacia el player con una
               // velocidad de rotación X. Tras el punto de apuntado ya deja de girar.

               if(!EnemyContext.enemyAnimator.GetBool(SwordsmanEnemy.DashSlashAttackHashId))
                    TriggerAttack();
          }
     }
     
     // Vamos a activar las variables de la Máquina de estados de animación (Animator) para que se triggeree la animación
     // de la acción que queremos.
     private void TriggerAttack()
     {
          EnemyContext.enemyAnimator.SetBool(SwordsmanEnemy.DashSlashAttackHashId, true);
          EnemyContext.enemyAnimator.Play(SwordsmanEnemy.DashSlashAttackHashId);
     }

     private IEnumerator DashAttackCoroutine()
     {
          float transcurredTime = 0.0f;
          
          // cada frame tiene que moverse X distancia, 
          // donde X es la distancia que se mueve en el dash total / el tiempo que dura el movimiento del dash
          float speedPerSecond = EnemyContext.DashAttackMovementDistance / EnemyContext.DashAttackDuration;
          while (transcurredTime < EnemyContext.DashAttackDuration)
          {
               // primero avanza, desplazamos a nuestro enemigo hacia el frente según qué tanto tiempo ha pasado
               transform.position += transform.forward * speedPerSecond * Time.deltaTime;
               transcurredTime += Time.deltaTime; // aumentamos nuestro contador de tiempo.
               // y que luego se espera hasta el siguiente frame
               yield return null;
          }
     }
     
     private IEnumerator RotateTowardsPlayerCoroutine()
     {
          while (true)
          {
               transform.LookAt(EnemyContext.GetTargetPlayerPosition());
               yield return null;
          }
     }
}


public class UltimateMeleeAttackState : EnemyBaseState
{
     // Cantidad de daño que tiene que recibir el enemigo durante este estado para pasar a staggered. (interrumpido)
     public float damageToStagger = 5;
     private float _currentDamageTaken = 0;
     private int _enemyHpWhenEnteringState;
     private float _randomTimeBeforeSimulatingDamage = 0;
     private float _transcurredTime = 0;
     
     void Awake()
     {
          stateName = "Ultimate Melee";
     }

     public override void OnEnter()
     {
          base.OnEnter();
          _currentDamageTaken = 0;
          _enemyHpWhenEnteringState = EnemyContext.CurrentHp;
          _randomTimeBeforeSimulatingDamage = Random.Range(0.5f, 2.0f); // más o menos lo que dura la animación del ultimate.
          _transcurredTime = 0;
     }
     
     public override void OnFixedUpdate()
     {
          // PARCHESOTE: No hacer nada si está staggereado.
          if (EnemyContext.enemyAnimator.GetBool(SwordsmanEnemy.UltimateSlashStaggeredHashId))
          {
               return;
          }
          
          EnemyContext.GoToTargetPlayer();

          // Este if es para solo acumular tiempo cuando sí se está ejecutando el ataque.
          if (EnemyContext.enemyAnimator.GetBool(SwordsmanEnemy.UltimateSlashAttackHashId))
          {
               _transcurredTime += Time.deltaTime;
               if (_transcurredTime >= _randomTimeBeforeSimulatingDamage)
               {
                    Debug.Log($"Enemigo tomando {damageToStagger} de daño simulado para el stagger porque" +
                              $" el _randomTimeBeforeSimulatingDamage es de {_randomTimeBeforeSimulatingDamage} y" +
                              $" _transcurredTime fue: {_transcurredTime}");
                    EnemyContext.TakeDamage((int)damageToStagger+1);
                    _transcurredTime = 0;
               }
          }
          

          
          // Si ha sufrido al menos damageToStagger cantidad de daño, entonces se staggerea.
          if (_enemyHpWhenEnteringState > EnemyContext.CurrentHp + damageToStagger)
          {
               Debug.Log("Stagger debería activarse");
               EnemyContext.enemyAnimator.SetBool(SwordsmanEnemy.UltimateSlashAttackHashId, false);
               EnemyContext.enemyAnimator.SetBool(SwordsmanEnemy.UltimateSlashStaggeredHashId, true);
               // EN TEORÍA, como mi función FinishedAttack del swordsman ya me manda a seleccionar el siguiente estado,
               // yo no tengo que hacer el ChangeState manualmente aquí.
               // OwnerFsm.ChangeState<BasicMeleeAttackState>(); // Por ahora le diré que se vaya al basic, pero podría ser otra cosa.
               return;
          }
          
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
          EnemyContext.enemyAnimator.Play(SwordsmanEnemy.UltimateSlashAttackHashId);
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