using UnityEngine;

public class PatrollerFSM : BaseFSM
{
    // Se necesita hacer el override para que esto se ejecute durante el start de tu máquina de estados.
    protected override void InitializeFSM()
    {
        // Esto hace que se le añada el componente, se le dice al estado que esta FSM es su dueña,
        // y se añade al diccionario de estados de la máquina de estados.
        AddState<ChaseState>();
        AddState<AttackState>();
    }

    protected override BaseState GetInitialState()
    {
        PatrolState initialState = AddState<PatrolState>();
        
        return initialState;
    }
}
