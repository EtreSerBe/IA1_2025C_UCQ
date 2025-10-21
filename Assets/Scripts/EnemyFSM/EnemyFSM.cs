using UnityEngine;

public class EnemyFSM : BaseFSM
{

    // Variables que le pueden importar a los estados del dueño de esta máquina de estados.
    // private BaseEnemy _enemyContext;
    
    // Se necesita hacer el override para que esto se ejecute durante el start de tu máquina de estados.
    protected override void InitializeFSM()
    {
        // Esto hace que se le añada el componente, se le dice al estado que esta FSM es su dueña,
        // y se añade al diccionario de estados de la máquina de estados.
        AddState<RangedState>();
    }

    protected override BaseState GetInitialState()
    {
        MeleeState initialState = AddState<MeleeState>();
        
        return initialState;
    }
}
