using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class JoinRoomMenu : Menu
{
    [SerializeField] private VisualTreeAsset roomListItem;

    public static JoinRoomMenu Instance;
    public Button backButton;
    public Button joinRoomButton;

    private RoomInfoListController roomInfoListController;

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
        joinRoomButton = root.Q<Button>("JoinRoomButton");

        joinRoomButton.SetEnabled(false);

        backButton.clickable.clicked += OnBackButtonClicked;

        roomInfoListController = new RoomInfoListController();
        roomInfoListController.InitializeRoomInfoList( root , roomListItem );
    }

    private void OnBackButtonClicked()
    {
        MenuManager.Instance.OpenMenu("PlayMenu");
        AudioManager.Instance.PlayButtonClickSfx();
    }

    public void RefreshRoomInfoList()
    {
        if( roomInfoListController != null )
        {
            roomInfoListController.RefreshRoomInfoList();
        }
    }
    
}