using UnityEngine;

public class WaypointDebug : MonoBehaviour
{
    [SerializeField] private int _wayPointIndex = 1;
    [SerializeField] private int _moveDirection = 1;
    [SerializeField] private int _waypointLength = 3;
    [SerializeField] private float _interval = 4f;

    private float _timer;
    private int _step;

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _interval)
        {
            _timer = 0f;
            SimulateStep();
        }
    }

    private void SimulateStep()
    {
        _step++;
        string condition = "X";
        int before = _moveDirection;

        if (_wayPointIndex == 0 || _wayPointIndex == _waypointLength - 1)
        {
            _moveDirection *= -1;
            condition = "O";
        }

        _wayPointIndex += _moveDirection;
        string note = condition == "O" ? "Balik arah" : "Jalan lanjut";

        Debug.Log($"Langkah {_step:D2} | Kondisi Ujung: {condition} | Arah: {before} --> {_moveDirection} | {note} | Index: {_wayPointIndex}");
    }
}
