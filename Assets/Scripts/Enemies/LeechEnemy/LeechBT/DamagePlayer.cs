using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehTree;

public class DamagePlayer : Node
{
    Vector3 _offset = Vector3.zero;
    GameObject _player;
    GameObject _leech;

    float timeRemaining = 0f;
    float _drainTime;
    int _damage;

    public DamagePlayer(GameObject player, GameObject leech, float drainTime, int dmg)
    {
        _player = player;
        _leech = leech;
        _drainTime = drainTime;
        _damage = dmg;
    }

    public override NodeState Evaluate()
    {
        //attaches the leech to the player -- leech moves with the player, is offset from player's position by set amount
        if (_offset == Vector3.zero || _leech.transform.parent != _player.transform)
        {
            _leech.transform.parent = _player.transform;
            //sets the leech's offset from the player
            _offset = (_leech.transform.position - _leech.GetComponent<LeechManager>().getPlayerPos().position) * 0.95f;
            _leech.transform.position = _leech.GetComponent<LeechManager>().getPlayerPos().position + _offset;
        }
        else
        {
            _leech.transform.position = _leech.GetComponent<LeechManager>().getPlayerPos().position + _offset;
        }

        //deals damage to the player while the leech is attached within set intervals
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
        else
        {
            _player.GetComponent<PlayerHealth>().TakeDamage(_damage); //player takes damage
            timeRemaining = _drainTime; //reset drain timer
        }

        state = NodeState.RUNNING;
        return state;
    }
}
