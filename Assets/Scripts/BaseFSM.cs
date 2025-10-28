using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseFSM : MonoBehaviour
{
    // El estado que está ejecutando actualmente la máquina de estados
    private BaseState _currentState;

    private Dictionary<Type, BaseState> _states = new Dictionary<Type, BaseState>();
    protected Dictionary<Type, BaseState> States => _states;
    
    // Los Update de la máquina de estados ÚNICAMENTE mandan a llamar el update del estado actual.
    private void FixedUpdate()
    {
        if(_currentState)
            _currentState.OnFixedUpdate();
    }

    private void Update()
    {
        if(!_currentState)
            _currentState.OnUpdate();
    }

    protected T AddState<T>() where T : BaseState
    {
        // No se debe usar "new" con clases que heredan de monobehaviour
        // PatrolState newPatrolState = new PatrolState();
        T newState = gameObject.AddComponent<T>();
        
        // Le seteas a dicho estado que esta máquina de estados es su dueña.
        newState.SetFsm(this);
        
        _states.Add(typeof(T), newState);
        return newState;
    }
    
    // Es pública porque necesitamos que la mande a llamar desde un State.
    // Los estados le piden a la máquina de estados que cambie a un nuevo estado.
    public void ChangeState<T>() where T : BaseState
    {
        if (!_states.TryGetValue(typeof(T), out var newState))
        {
            Debug.LogError($"GameObject {gameObject.name} does not have a state of type " +
                           $"{typeof(T).Name} in their FSM. Change state failed.");
            return;
        }
            
        // Cuando un estado va a salir de ejecución, debe mandar a llamar su función OnExit().
        _currentState.OnExit();
        
        // Ponemos el estado nuevo como el estado actual
        _currentState = newState;
        
        // Está entrando a este nuevo estado, entonces llamamos su OnEnter()
        _currentState.OnEnter();
    }

    protected virtual void InitializeFSM()
    {
        // No hace nada porque es la clase base.
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Hace que el estado inicial de la FSM sea el estado actual
        _currentState = GetInitialState();
        if (_currentState == null)
        {
            Debug.LogError($"El gameObject: {gameObject.name} tiene como null su estado inicial." +
                           $"¿Tal vez olvidaste hacer el override de la función GetInitialState en tu clase " +
                           $"que hereda de BaseFSM?");
        }
        
        _currentState.OnEnter();
        // Se encarga de la configuración inicial necesaria para cada máquina de estados específica.
        InitializeFSM();
    }

    // Regresa null solamente la de BaseFSM para saber que no se cambió (no se hizo el override) en las
    // clases hijas.
    protected virtual BaseState GetInitialState()
    {
        return null;
    }

    private void OnGUI()
    {
        string content = _currentState == null ? "NO CURRENT STATE" : _currentState.StateName;
        GUILayout.Label($"<color='black'><size=40>{content}</size></color>");
    }
}
