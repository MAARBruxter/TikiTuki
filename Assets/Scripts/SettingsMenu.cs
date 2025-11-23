using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;

    public TMP_Text inputDeviceText;

    private void Start()
    {
        // Loads the saved volumen
        if (PlayerPrefs.HasKey(GameConstants.MUSICVOLUME_KEY))
            musicSlider.value = PlayerPrefs.GetFloat(GameConstants.MUSICVOLUME_KEY);
        else
            musicSlider.value = 1f;

        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
    }

    private void Update()
    {
        UpdateDeviceText();
    }

    /// <summary>
    /// It changes the volume of the game.
    /// </summary>
    /// <param name="value"></param>
    private void OnMusicVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(value);
    }

    /// <summary>
    /// Indicates which controller is the player using.
    /// </summary>
    void UpdateDeviceText()
    {
        if (Keyboard.current != null && Keyboard.current.anyKey.isPressed)
        {
            inputDeviceText.text = "KeyBoard";
        }
        //TODO check Gamepad active with all controls correctly
        else if (Gamepad.current != null && Gamepad.current.leftStick.ReadValue().magnitude > 0)
        {
            inputDeviceText.text = "GamePad";
        }
    }

    private void OnDestroy()
    {
        musicSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
    }
}
