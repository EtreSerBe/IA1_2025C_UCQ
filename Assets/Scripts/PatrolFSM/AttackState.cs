using System.Collections;
using UnityEngine;

public class AttackState : BaseState
{
    private void Awake()
    {
        stateName = "Attack";
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
        OwnerFsm.ChangeState<PatrolState>();
    }
}
