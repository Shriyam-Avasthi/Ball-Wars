using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CreateRoomMenu : Menu
{
    public static CreateRoomMenu Instance;
    public Button backButton;
    public Button createRoomButton;
    public TextField roomNameField;

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

        backButton = root.Q<Button>("BackButton");
        createRoomButton = root.Q<Button>("CreateRoomButton");
        roomNameField = root.Q<TextField>("RoomNameField");

        backButton.clickable.clicked += OnBackButtonClicked;
        createRoomButton.clickable.clicked += OnCreateRoomButtonClicked;
    }

    private void OnBackButtonClicked()
    {
        MenuManager.Instance.OpenMenu("PlayMenu");
        AudioManager.Instance.PlayButtonClickSfx();
    }

    private void OnCreateRoomButtonClicked()
    {
        MasterClient.Instance.SendToServer( new NetCreateRoom(roomNameField.text) );
        AudioManager.Instance.PlayButtonClickSfx();
        MenuManager.Instance.OpenMenu("Loading");
    }
}