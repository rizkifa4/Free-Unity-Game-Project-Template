using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class EnemyAnimation : MonoBehaviour
{
    private Animator _anim;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    public void SetAnimState(string stateName, bool value)
    {
        _anim.SetBool(stateName, value);
    }

    public void SetAnimBlend(string parameterName, float value)
    {
        _anim.SetFloat(parameterName, value);
    }

    public void SetAnimTrigger(string parameterName)
    {
        _anim.SetTrigger(parameterName);
    }
}
