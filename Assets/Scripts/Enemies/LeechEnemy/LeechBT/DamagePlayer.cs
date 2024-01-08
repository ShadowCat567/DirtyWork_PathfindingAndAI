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
    int _impactDmg;

    bool _impactDmgDealt = false;

    public DamagePlayer(GameObject player, GameObject leech, float drainTime, int dmg)
    {
        _player = player;
        _leech = leech;
        _drainTime = drainTime;
        _damage = dmg;
        _impactDmg = dmg;
    }

    public override NodeState Evaluate()
    {
        if (_offset == Vector3.zero || _leech.transform.parent != _player.transform)
        {
            _leech.transform.parent = _player.transform;
            _offset = (_leech.transform.position - _leech.GetComponent<LeechManager>().getPlayerPos().position) * 0.95f;
            _leech.transform.position = _leech.GetComponent<LeechManager>().getPlayerPos().position + _offset;

            //player takes impact damage when leech hits them
            if (!_impactDmgDealt)
            {
                //_player.GetComponent<PlayerHealth>().TakeDamage(_impactDmg);
                _impactDmgDealt = true;
            }
        }
        else
        {
            _leech.transform.position = _leech.GetComponent<LeechManager>().getPlayerPos().position + _offset;
        }

        //timer that deals damage to the player while the leech is attached
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
        else
        {
            //_player.GetComponent<PlayerHealth>().TakeDamage(_damage);
            timeRemaining = _drainTime;
        }

        state = NodeState.RUNNING;
        return state;
    }
}
