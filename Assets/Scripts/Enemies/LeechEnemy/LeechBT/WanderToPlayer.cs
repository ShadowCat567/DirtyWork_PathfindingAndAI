using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehTree;

public class WanderToPlayer : Node
{
    //use pathfinding to wander towards the player
    //does not update player position every frame, goes to last stored location of player before updating
    PathGraph _pfGraph;
    PathNode _pfCurNode;
    PathNode _pfEndNode;

    List<PathNode> pfPath = new List<PathNode>();
    int _index;

    GameObject _player = null;
    PlayerMovement _pm;
    GameObject _leechTrans;
    LeechManager _lm;
    float _idleVel;

    bool positionChanged = false;

    public WanderToPlayer(PathGraph pfGraph, PathNode pfCurNode, GameObject player, GameObject leech, float idleVelo,
        PathNode endNode, int index)
    {
        _pfGraph = pfGraph;
        _leechTrans = leech;
        _idleVel = idleVelo;
        _pfEndNode = endNode;

        _index = index;
    }

    public override NodeState Evaluate()
    {
        if (!_player) //if value of player has not been initalized, initalize it
        {
            _player = GameObject.Find("Player");
            _pm = _player.GetComponent<PlayerMovement>();
            _lm = _leechTrans.GetComponent<LeechManager>();
            _pfCurNode = _leechTrans.GetComponent<LeechManager>().getCurNode();
        }
        else
        {
            //reached the end of a path (or made an invalid path), need to find a new one
            if ((pfPath.Count == 0 || pfPath == null) || _index == pfPath.Count || positionChanged)
            {
                positionChanged = false;

                if (pfPath == null)
                {
                    pfPath = new List<PathNode>();
                }

                _index = 1;
                _pfEndNode = _pm.getPlayerNode(); //player's position -- translate player's position to graph node position
                pfPath = _pfGraph.findPath(_pfCurNode, _pfEndNode);
                _pfCurNode = pfPath[_index];
            }
            else
            {
                //move to next position in the path
                if ((Vector2)_leechTrans.transform.position != _pfCurNode.getLocation())
                {
                    _leechTrans.transform.position = Vector2.MoveTowards(_leechTrans.transform.position, _pfCurNode.getLocation(), _idleVel * Time.deltaTime);
                }
                else
                {
                    if (_index < pfPath.Count - 1) // if we have not reach the end of the path, go to the next nodes
                    {
                        _index += 1;
                        _pfCurNode = pfPath[_index];
                    }
                    else if (_index == pfPath.Count - 1) //if we have reached the end of the path, find new player position
                    {
                        _pfCurNode = _pfEndNode;
                        positionChanged = true;
                    }
                }
            }
        }

        state = NodeState.RUNNING;
        return state;
    }
}
