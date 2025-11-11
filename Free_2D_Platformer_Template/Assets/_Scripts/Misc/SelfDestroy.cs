using UnityEngine;

/// <summary>
/// A reusable component that allows any object to destroy itself.
/// Typically called through an Animation Event on the final frame of an animation.
/// </summary>
public class SelfDestroy : MonoBehaviour
{
    /// <summary>
    /// Destroys this <see cref="GameObject"/>.  
    /// Commonly invoked from an Animation Event (e.g., at the end of an animation).
    /// </summary>
    private void DestroyItself() => Destroy(gameObject);
}
