using System;
using UnityEngine;

public class IdleState : BaseState
{

    // Sale de idle cuando le pegan o cuando ve al jugador

    private void OnCollisionEnter(Collision other)
    {
        // Checamos si fue alguien en una layer que le hace daño al enemigo o fue el player tal cual.
        if (other.gameObject.layer == Constants.Layers.PlayerBulletLayer ||
            other.gameObject.layer == Constants.Layers.PlayerLayer)
        {
            // Entonces nos salimos del estado idle. Le decimos a la máquina de estados que nos mande
            // al estado correspondiente.
            // OwnerFsm.ChangeState<MeleeState>();
            return; // SIEMPRE poner return o su equivalente después de un ChangeState.
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Checamos si fue alguien en una layer que le hace daño al enemigo o fue el player tal cual.
        if (other.gameObject.layer == Constants.Layers.PlayerBulletLayer ||
            other.gameObject.layer == Constants.Layers.PlayerLayer)
        {
            // Entonces nos salimos del estado idle. Le decimos a la máquina de estados que nos mande
            // al estado correspondiente.
            // OwnerFsm.ChangeState<MeleeState>();
            return; // SIEMPRE poner return o su equivalente después de un ChangeState.
        }
    }

    public override void OnFixedUpdate()
    {
        // Checar si este enemigo encontró al player.
        // EnemyFSM ownerEnemyFsm = (EnemyFSM)OwnerFsm;
        // ownerEnemyFsm.EnemyContext.GetSensesSystem().
    }

    void Awake()
    {
        stateName = "Idle";
    }
}
