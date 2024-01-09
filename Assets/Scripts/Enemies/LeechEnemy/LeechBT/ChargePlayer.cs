using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehTree;

public class ChargePlayer : Node
{
    Vector3 playerPos;
    Vector3 targetPos;
    bool playerInLOS = false;

    bool startedMoving = false;

    bool waiting = false;
    float waitCounter = 0.0f;
    float waitTime = 0.6f;

    GameObject _player;
    //PlayerMovement _pm;
    GameObject _leech;
    float _chargeVelo;

    float _lowSpeed = 2f;
    float _highSpeed = 10f;
    float _soapSpeed = 4f;

    float acceleration = 0.5f;
    float decceleration = 4f;

    LeechManager _lm;
    Transform _originPar;

    PathGraph _pfGraph;
    PathNode _pfCurNode;
    PathNode _pfEndNode;

    List<PathNode> pfPath = new List<PathNode>();
    int _index;
    float _idleVel;

    public ChargePlayer(GameObject player, GameObject leech, float lowVelo, float highVelo, Transform originalParent,
        PathGraph pfGraph, PathNode pfCurNode, PathNode endNode, float idleVelo)
    {
        _player = player;
        _leech = leech;
        _lowSpeed = lowVelo;
        _highSpeed = highVelo;
        _chargeVelo = lowVelo;//Random.Range(3f, 6f);
        _originPar = originalParent;

        _pfGraph = pfGraph;
        _pfCurNode = pfCurNode;
        _pfEndNode = endNode;
        _index = 0;
        _idleVel = idleVelo;
    }

    public override NodeState Evaluate()
    {
        if (_leech.transform.parent == _player.transform)
        {
            _leech.transform.parent = _originPar;
        }

        if (!startedMoving)
        {
            _lm = _leech.GetComponent<LeechManager>();
            playerPos = _lm.getPlayerPos().position;
            _player = _lm.getPlayer();
            //_pm = _player.GetComponent<PlayerMovement>();
            targetPos = (playerPos - _leech.transform.position).normalized;
            //_leechRB.AddForce((playerPos - _leech.transform.position).normalized * _chargeVelo, ForceMode2D.Force);
            playerInLOS = true;
            startedMoving = true;
        }
        else
        {
            //this section needs to be rewritten
            if (waiting)
            {
                //if the leech is not moving, wait for a fixed amout of time, then update the player's position
                waitCounter += Time.deltaTime;

                _chargeVelo -= decceleration;
                _chargeVelo = Mathf.Clamp(_chargeVelo, _lowSpeed, _highSpeed);

                if (waitCounter >= waitTime)
                {
                    //fire raycast in direction of player
                    //if player is not hit, move via pathfinding until player able to hit player
                    //then charge
                    targetPos = (playerPos - _leech.transform.position).normalized;
                    playerInLOS = CanSeePlayer(_leech.transform.position, targetPos);

                    waitCounter = 0f;
                    waiting = false;
                }
            }
            else
            {
                if (playerInLOS)
                {
                    //charge player
                    //launch leech in direction of player
                    _chargeVelo += acceleration;
                    _chargeVelo = Mathf.Clamp(_chargeVelo, _lowSpeed, _highSpeed);
                    
                    _leech.transform.position += targetPos * _chargeVelo * Time.deltaTime;
                    pfPath.Clear();
                }
                else
                {
                    //pathfind until we can charge
                    if (pfPath == null || pfPath.Count == 0)
                    {
                        playerPos = _lm.getPlayerPos().position;
                        targetPos = (playerPos - _leech.transform.position).normalized;
                        playerInLOS = CanSeePlayer(_leech.transform.position, targetPos);

                        if (pfPath == null)
                        {
                            pfPath = new List<PathNode>();
                        }

                        pfPath.Clear();

                        _index = 1;
                        _pfCurNode = _lm.getLeechNode();
                        //_pfEndNode = _pm.getPlayerNode();//player's position -- translate player's position to graph node position
                        pfPath = _pfGraph.findPath(_pfCurNode, _pfEndNode);
                        _pfCurNode = pfPath[_index];
                    }
                    else
                    {
                        playerPos = _lm.getPlayerPos().position;
                        targetPos = (playerPos - _leech.transform.position).normalized;
                        playerInLOS = CanSeePlayer(_leech.transform.position, targetPos);

                        //move to next position in the path
                        if ((Vector2)_leech.transform.position != _pfCurNode.getLocation())
                        {
                            _leech.transform.position = Vector2.MoveTowards(_leech.transform.position, _pfCurNode.getLocation(), _idleVel * Time.deltaTime);
                        }
                        else
                        {
                            if (playerInLOS)
                            {
                                _chargeVelo += acceleration;
                                _chargeVelo = Mathf.Clamp(_chargeVelo, _lowSpeed, _highSpeed);

                                _leech.transform.position += targetPos * _chargeVelo * Time.deltaTime;
                                pfPath.Clear();
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

                                    playerPos = _lm.getPlayerPos().position;
                                    targetPos = (playerPos - _leech.transform.position).normalized;
                                    playerInLOS = CanSeePlayer(_leech.transform.position, targetPos);

                                    pfPath.Clear();
                                    //lm.setFacing(lm.facingDirection(lm.transform.position, lm.getCurNode().getLocation()));
                                }
                            }
                        }
                    }
                }
            }

            if (_leech.GetComponent<LeechManager>().collided)
            {
                //Debug.Log("collided");
                _leech.GetComponent<LeechManager>().collided = false;
                waiting = true;
                playerPos = _leech.GetComponent<LeechManager>().getPlayerPos().position;
                targetPos = (playerPos - _leech.transform.position).normalized;
            }
        }

        state = NodeState.RUNNING;
        return state;
    }


    bool CanSeePlayer(Vector3 enemyPos, Vector3 target)
    {
        RaycastHit2D ray = Physics2D.Raycast(enemyPos, target, 500f, LayerMask.GetMask("Player", "LevelObj"));
        //Debug.DrawRay(enemyPos, target, Color.magenta);
        return RayCollidedWithPlayer(ray);
    }

    bool RayCollidedWithPlayer(RaycastHit2D ray)
    {
        if (ray.collider != null && ray.collider.gameObject.layer == 6)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
