using System;
using UnityEngine;

public class EnemyFSM : BaseFSM
{

    // Variables que le pueden importar a los estados del dueño de esta máquina de estados.
    private BaseEnemy _enemyContext;
    public BaseEnemy EnemyContext => _enemyContext;

    private void Awake()
    {
        if (!TryGetComponent(out _enemyContext))
        {
            Debug.LogError($"Failed to get BaseEnemy component from gameObject: {gameObject.name}");
        }
    }

    // Se necesita hacer el override para que esto se ejecute durante el start de tu máquina de estados.
    protected override void InitializeFSM()
    {
        // Esto hace que se le añada el componente, se le dice al estado que esta FSM es su dueña,
        // y se añade al diccionario de estados de la máquina de estados.
        AddState<RangedState>();
        
        AddState<AreaMeleeAttackState>();
        AddState<DashMeleeAttackState>();
        AddState<UltimateMeleeAttackState>();
        
    }
    

    protected override BaseState GetInitialState()
    {
        BasicMeleeAttackState initialState = AddState<BasicMeleeAttackState>();
        // MeleeAttackState initialState = AddState<MeleeAttackState>(); // estado de muestra de cómo factorizar estados.
       
        return initialState;
    }
    
}
