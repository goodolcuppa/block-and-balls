using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public Button musicToggle;
    public Image musicToggleImage;
    public Sprite muteIcon;
    public Sprite unmuteIcon;
    public GameObject mainPanel;
    public Button inventoryToggle;
    public GameObject inventoryPanel;
    public Button controlsToggle;
    public GameObject controlsPanel;
    private bool controlsEnabled;
    public Button quitButton;
    public TextMeshProUGUI moneyText;

    public AudioSource audioSource;

    void Start()
    {
        controlsEnabled = false;

        musicToggle.onClick.AddListener(ToggleMusic);
        controlsToggle.onClick.AddListener(ToggleControls);
        quitButton.onClick.AddListener(OnQuitClick);

        moneyText.text = PlayerPrefs.GetInt("Money", 0).ToString();

        audioSource = Camera.main.GetComponent<AudioSource>();
        audioSource.mute = PlayerPrefs.GetInt("isMuted", 0) == 1 ? true : false;

        SetMuteImage();
    }

    private void ToggleMusic()
    {
        audioSource.mute = !audioSource.mute;
        if (audioSource.mute)
        {
            PlayerPrefs.SetInt("isMuted", 1);
        }
        else
        {
            PlayerPrefs.SetInt("isMuted", 0);
        }
        SetMuteImage();
    }

    private void SetMuteImage()
    {
        musicToggleImage.sprite = audioSource.mute ? unmuteIcon : muteIcon;
    }

    private void ToggleControls()
    {
        controlsEnabled = !controlsEnabled;
        controlsPanel.gameObject.SetActive(controlsEnabled);
    }

    private void OnQuitClick()
    {
        Application.Quit();
    }
}
