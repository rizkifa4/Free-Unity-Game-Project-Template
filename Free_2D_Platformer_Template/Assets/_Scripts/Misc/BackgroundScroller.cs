using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class BackgroundScroller : MonoBehaviour
{
    [Header("Move Direction")]
    [SerializeField, Range(-1f, 1f)] private float _xSpeedDirection;
    [SerializeField, Range(-1f, 1f)] private float _ySpeedDirection;

    [Header("Background Color")]
    [SerializeField] private BackgroundType _backgroundType;
    [SerializeField] private Texture2D[] _textures;
    private Material _material;

    private void Awake()
    {
        _material = GetComponent<MeshRenderer>().material;
        UpdateBackgroundTexture();
    }

    private void UpdateBackgroundTexture()
    {
        _material.mainTexture = _textures[(int)_backgroundType];
    }

    private void Update()
    {
        Vector2 moveDirection = new(_xSpeedDirection, _ySpeedDirection);
        Vector2 moveDelta = moveDirection * Time.deltaTime;
        _material.mainTextureOffset += moveDelta;
    }
}

public enum BackgroundType { Blue, Brown, Gray, Green, Pink, Purple, Yellow }