using UnityEngine;

public class Fruit : MonoBehaviour
{
    [SerializeField] private FruitType _fruitType;
    [SerializeField] private GameObject _pickupVFX;

    private FruitAnimation _fruitAnimation;
    private GameManager _gameManager;
    private bool _isCollected;

    private void Awake()
    {
        _fruitAnimation = GetComponentInChildren<FruitAnimation>();
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
        SetAnimLookIfNeeded();
    }

    private void SetAnimLookIfNeeded()
    {
        if (!_gameManager.IsFruitRandomLook)
        {
            SetFruitVisual();
            return;
        }

        int randomIndex = Random.Range(0, 8);
        _fruitAnimation.SetAnimBlend("FruitIndex", randomIndex);
    }

    private void SetFruitVisual()
    {
        _fruitAnimation.SetAnimBlend("FruitIndex", (int)_fruitType);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out _) && !_isCollected)
        {
            _isCollected = true;
            _gameManager.AddFruit(1);

            GameObject newFX = Instantiate(_pickupVFX, transform.position, Quaternion.identity);

            // Animator vfxAnim = newFX.GetComponent<Animator>();
            // AnimationClip clip = vfxAnim.runtimeAnimatorController.animationClips[0];
            // Destroy(newFX, clip.length);

            Destroy(gameObject);
        }
    }
}

public enum FruitType
{
    Apple,
    Banana,
    Cherry,
    Kiwi,
    Melon,
    Orange,
    Pineapple,
    Strawberry
}
