using UnityEngine;


// no lo vamos a asignar como componente manualmente, pero sí necesitamos que herede de Monobehavior
// para poder usar corrutinas. Los asignamos a través de código en la FSM con AddComponent.
public class BaseState : MonoBehaviour
{
    // Protected es que esta clase y las clases que hereden de ella pueden acceder a dichas variables y funciones
    protected string stateName = "BaseState";
    public string StateName => stateName;

    protected BaseFSM OwnerFsm;

    public virtual void SetFsm(BaseFSM owner)
    {
        OwnerFsm = owner;
    }
    
    // virtual quiere decir que es una función que puede cambiar (override), es decir, puede hacer
    // cosas distintas que en la clase base.
    
    // Inicializaciones (Crear variables, pedir memoria)
    public virtual void OnEnter()
    {
        Debug.Log($"On Enter: {stateName}");
    }

    // Manejar eventos (input, físicas, lógica de juego)
    public virtual void OnUpdate()
    {
        // Debug.Log($"On Update: {stateName}");
    }
    
    // Manejar eventos (físicas)
    public virtual void OnFixedUpdate()
    {
        // Debug.Log($"On FixedUpdate: {stateName}");
    }

    // Limpieza (Liberar memoria, desactivar variables)
    public virtual void OnExit()
    {
        Debug.Log($"On Exit: {stateName}");
    }
    
}
