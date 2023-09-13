using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayMenu : Menu
{

    public static PlayMenu Instance;
    public TextField playerNameField;
    public Button createRoomButton;
    public Button joinRoomButton;
    public Button backButton;

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

        createRoomButton = root.Q<Button>("CreateRoomButton");
        joinRoomButton = root.Q<Button>("JoinRoomButton");
        backButton = root.Q<Button>("BackButton");
        playerNameField = root.Q<TextField>("PlayerNameField");

        createRoomButton.clickable.clicked += OnCreateRoomButtonClicked;
        joinRoomButton.clickable.clicked += OnJoinRoomButtonClicked;
        backButton.clickable.clicked += OnBackButtonClicked;
    }

    private void OnCreateRoomButtonClicked()
    {
        MenuManager.Instance.OpenMenu("CreateRoom");
        AudioManager.Instance.PlayButtonClickSfx();
    }

    private void OnJoinRoomButtonClicked()
    {
        MenuManager.Instance.OpenMenu("JoinRoom");
        AudioManager.Instance.PlayButtonClickSfx();
    }
    private void OnBackButtonClicked()
    {
        MenuManager.Instance.OpenMenu("MainMenu");
        AudioManager.Instance.PlayButtonClickSfx();
    }

}

