using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehTree;

public class WanderToPlayer : Node
{
    //use pathfinding to wander towards the player
    //does not update player position every frame, waits for a bit before updating player's position
    PathGraph _pfGraph;
    PathNode _pfCurNode;
    PathNode _pfEndNode;

    PathNode _playerNode;

    List<PathNode> pfPath = new List<PathNode>();
    int _index;

    GameObject _player = null;
    //PlayerMovement _pm;
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
        if (!_player)
        {
            _player = GameObject.Find("Player");
            //_pm = _player.GetComponent<PlayerMovement>();
            _lm = _leechTrans.GetComponent<LeechManager>();
            //pfPath = new List<PathNode>();
            _pfCurNode = _leechTrans.GetComponent<LeechManager>().getCurNode();
            //Debug.Log("Path exists: " + pfPath);
        }
        else
        {
            /*
            for(int i = 0; i < lm.getPath().Count; i ++)
            {
                lm.getPath()[i].GetComponent<SpriteRenderer>().color = new Color(0.1f, 0.1f, 0.1f, 0.2f);
            }
            */
            //reached the end of a path, need to find a new one
            if ((pfPath.Count == 0 || pfPath == null) || _index == pfPath.Count || positionChanged)
            {
                //pfPath.Clear();
                positionChanged = false;

                if (pfPath == null)
                {
                    pfPath = new List<PathNode>();
                }

                _index = 1;
                //_pfEndNode = _pm.getPlayerNode();//player's position -- translate player's position to graph node position
                //Debug.Log("Path exists:" + pfPath);
                pfPath = _pfGraph.findPath(_pfCurNode, _pfEndNode);
                //Debug.Log("Path exists 2:" + pfPath);
                //Debug.Log("Path: " + pfPath.Count);
                _pfCurNode = pfPath[_index];
                //Debug.Log("CurNode: " + lm.getCurNode().getLocation() + " Path: " + lm.getPath().Count + " index: " + lm.index);
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
                    if (_index < pfPath.Count - 1)
                    {
                        _index += 1;
                        _pfCurNode = pfPath[_index];
                    }
                    else if (_index == pfPath.Count - 1)
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
