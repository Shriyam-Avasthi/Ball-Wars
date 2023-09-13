using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : Menu
{
    public static MainMenu Instance;
    public Button playButton;
    public TextField ipAdressTextField;
    public Button exitButton;

    private void Awake() {
        if( Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
        
    }

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        VisualElement root = uiDocument.rootVisualElement;

        playButton = root.Q<Button>("PlayButton");
        exitButton = root.Q<Button>("ExitButton");
        ipAdressTextField = root.Q<TextField>("IPAddressTextField");

        playButton.clickable.clicked += OnPlayButtonClicked;
        exitButton.clickable.clicked += OnExitButtonClicked;
    }

    private void OnPlayButtonClicked()
    {
        MenuManager.Instance.OpenMenu("PlayMenu");
        AudioManager.Instance.PlayButtonClickSfx();
        MasterServer.Instance.Init();
        MasterClient.Instance.Init();
    }

    private void OnExitButtonClicked()
    {
        Application.Quit();
        AudioManager.Instance.PlayButtonClickSfx();
    }

}

