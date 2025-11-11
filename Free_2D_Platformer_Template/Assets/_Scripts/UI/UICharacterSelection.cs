using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterSelection : MonoBehaviour
{
    [SerializeField] private Character[] _characterList;

    [Header("UI Character Info")]
    [SerializeField] private int _characterIndex;
    [SerializeField] private int _maxIndex;
    [SerializeField] private Animator _characterSelectionDisplay;
    [Space]
    [SerializeField] private Image _lockedImage;
    [SerializeField] private Sprite _unlockedSprite;
    [SerializeField] private Sprite _lockedSprite;
    [SerializeField] private TextMeshProUGUI _selectCharacterText;

    private UIMainMenu _uiMainMenu;

    private void Awake()
    {
        _uiMainMenu = GetComponentInParent<UIMainMenu>();
        LoadCharacterUnlock();
    }

    private void Start()
    {
        UpdateCharacterDisplay();
    }

    private void LoadCharacterUnlock()
    {
        for (int i = 0; i < _characterList.Length; i++)
        {
            string characterName = _characterList[i].CharacterName;
            bool isCharacterUnlock = PlayerPrefs.GetInt($"{characterName} Unlocked", 0) == 1;

            _characterList[i].IsUnlocked = isCharacterUnlock || i == 0;
        }
    }

    public void SelectCharacter()
    {
        if (!_characterList[_characterIndex].IsUnlocked)
        {
            OpenLockCharacter(_characterIndex);
        }
        else
        {
            CharacterManager.Instance.SetCharacterId(_characterIndex);
            _uiMainMenu.NewGame();
        }

        UpdateCharacterDisplay();
    }

    private void UpdateCharacterDisplay()
    {
        for (int i = 0; i < _characterSelectionDisplay.layerCount; i++)
        {
            _characterSelectionDisplay.SetLayerWeight(i, 0);
        }

        _characterSelectionDisplay.SetLayerWeight(_characterIndex, 1);

        bool isUnlocked = _characterList[_characterIndex].IsUnlocked;
        _lockedImage.sprite = isUnlocked ? _unlockedSprite : _lockedSprite;
        _selectCharacterText.text = isUnlocked ? "Select" : "Unlock";
    }

    private void OpenLockCharacter(int index)
    {
        if (_characterList[index].IsUnlocked) return;

        string characterName = _characterList[_characterIndex].CharacterName;
        _characterList[_characterIndex].IsUnlocked = true;

        PlayerPrefs.SetInt($"{characterName} Unlocked", 1);
    }

    public void NextCharacter()
    {
        _characterIndex++;

        if (_characterIndex > _maxIndex)
        {
            _characterIndex = 0;
        }

        UpdateCharacterDisplay();
    }

    public void PreviousCharacter()
    {
        _characterIndex--;

        if (_characterIndex < 0)
        {
            _characterIndex = _maxIndex;
        }

        UpdateCharacterDisplay();
    }
}

[Serializable]
public struct Character
{
    public string CharacterName;
    public bool IsUnlocked;
}