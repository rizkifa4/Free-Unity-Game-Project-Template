using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private AnimatorOverrideController[] _animators;
    [SerializeField] private AnimationClip _knockbackClip;
    public float KnockbackDuration { get; private set; }

    private Animator _anim;
    private Player _player;

    /// <summary>
    /// Initializes references and calculates knockback duration
    /// based on the assigned knockback animation clip.
    /// </summary>
    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _player = GetComponentInParent<Player>();
        KnockbackDuration = _knockbackClip.length;
    }

    /// <summary>
    /// Updates the player's animator override based on the active character
    /// selected in <see cref="CharacterManager"/>.
    /// </summary>
    /// <remarks>
    /// If no CharacterManager exists in the scene, the method safely exits.
    /// </remarks>
    public void UpdatePlayerLook()
    {
        CharacterManager characterManager = CharacterManager.Instance;
        if (!characterManager) return;

        _anim.runtimeAnimatorController = _animators[characterManager.CharacterId];
    }

    /// <summary>
    /// Sets a boolean parameter on the Animator.
    /// </summary>
    /// <param name="stateName">Name of the animator boolean parameter.</param>
    /// <param name="value">Boolean value to assign.</param>
    public void SetAnimState(string stateName, bool value)
    {
        _anim.SetBool(stateName, value);
    }

    /// <summary>
    /// Sets a float parameter on the Animator, typically for blend tree values.
    /// </summary>
    /// <param name="parameterName">Name of the float parameter.</param>
    /// <param name="value">Value to assign to the parameter.</param>
    public void SetAnimBlend(string parameterName, float value)
    {
        _anim.SetFloat(parameterName, value);
    }

    /// <summary>
    /// Activates a trigger parameter on the Animator.
    /// </summary>
    /// <param name="triggerName">Name of the trigger parameter.</param>
    public void SetAnimTrigger(string triggerName)
    {
        _anim.SetTrigger(triggerName);
    }

    /// <summary>
    /// Animation event callback that signals the end of the respawn sequence.
    /// </summary>
    /// <remarks>
    /// Invoked by an animation event. Re-enables player control and gravity
    /// through <see cref="Player.RespawnFinished(bool)"/>.
    /// </remarks>
    private void RespawnFinish()
    {
        _player.RespawnFinished(true);
    }
}
