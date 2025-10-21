using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PatrolState : BaseState
{
    void Awake()
    {
        stateName = "Patrol";
    }

    public override void OnEnter()
    {
        base.OnEnter();
        StartCoroutine(ChangeState());
    }

    private IEnumerator ChangeState()
    {
        yield return new WaitForSeconds(3.0f);
        
        // Le pide a la máquina de estados que ahora cambie a otro estado.
        OwnerFsm.ChangeState<ChaseState>();
        // SIEMPRE Después de un change State, el estado debe de hacer un return
        // (bueno, salirse de donde está y no ejecutar absolutamente nada más).
        yield break;
        
        // Demostración del problema. A pesar de que ya se había mandado a llamar el OnExit de este estado
        // se siguió ejecutando código de dicho estado.
        Debug.LogWarning("Estoy en el estado de Patrol");
    }
    
}
