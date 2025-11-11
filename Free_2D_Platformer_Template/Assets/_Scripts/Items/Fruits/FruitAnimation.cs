using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FruitAnimation : MonoBehaviour
{
    private Animator _anim;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    public void SetAnimBlend(string parameterName, float value)
    {
        _anim.SetFloat(parameterName, value);
    }
}
