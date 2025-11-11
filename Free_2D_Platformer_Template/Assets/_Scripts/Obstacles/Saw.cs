using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class Saw : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5;
    [SerializeField] private float _cooldown = 1;
    [SerializeField] private Transform[] _waypoints = new Transform[0];
    private Vector3[] _waypointPosition;
    [SerializeField] private int _wayPointIndex = 1;
    [SerializeField] private int _moveDirection = 1;
    private bool _canMove = true;

    private Animator _anim;
    private SpriteRenderer _sr;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();

        InitReferences();
    }

    private void InitReferences()
    {
        UpdateWaypoint();
        transform.position = _waypointPosition[0];
    }

    private void UpdateWaypoint()
    {
        List<SawWaypoint> waypointsList = new(GetComponentsInChildren<SawWaypoint>());

        if (waypointsList.Count != _waypoints.Length)
        {
            _waypoints = new Transform[waypointsList.Count];

            for (int i = 0; i < waypointsList.Count; i++)
            {
                _waypoints[i] = waypointsList[i].transform;
            }
        }

        _waypointPosition = new Vector3[_waypoints.Length];

        for (int i = 0; i < _waypoints.Length; i++)
        {
            _waypointPosition[i] = _waypoints[i].position;
        }
    }

    private void Update()
    {
        _anim.SetBool("Active", _canMove);

        if (!_canMove) return;

        transform.position =
            Vector2.MoveTowards(
                transform.position,
                _waypointPosition[_wayPointIndex],
                _moveSpeed * Time.deltaTime
            );

        if (Vector2.Distance(transform.position, _waypointPosition[_wayPointIndex]) < .1f)
        {
            if (_wayPointIndex == _waypointPosition.Length - 1 || _wayPointIndex == 0)
            {
                _moveDirection *= -1;
                StartCoroutine(StopMovement(_cooldown));
            }

            _wayPointIndex += _moveDirection;
        }
    }

    private IEnumerator StopMovement(float delay)
    {
        _canMove = false;
        yield return new WaitForSeconds(delay);
        _canMove = true;
        _sr.flipX = !_sr.flipX;
    }
}
