using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LobbyMenu : Menu
{
    [SerializeField] private VisualTreeAsset playerListItem;

    public static LobbyMenu Instance;
    public Button backButton;
    public Button startGameButton;

    private PlayerInfoListController playerInfoListController;

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
        startGameButton = root.Q<Button>("StartGameButton");
        startGameButton.SetEnabled(false);

        startGameButton.clickable.clicked += OnStartGameButtonClicked;
        backButton.clickable.clicked += OnBackButtonClicked;

        playerInfoListController = new PlayerInfoListController();
        playerInfoListController.InitializePlayerInfoList( root , playerListItem );

    }

    public void RefreshPlayerInfoList()
    {
        if( playerInfoListController != null )
        {
            playerInfoListController.RefreshPlayerInfoList();
        }
    }

    private void OnBackButtonClicked()
    {
        MenuManager.Instance.OpenMenu("PlayMenu");
        AudioManager.Instance.PlayButtonClickSfx();
        // Disconnect the previous Connection               :: TODO
        Client.Instance.DisconnectFromServer();
    }

    private void OnStartGameButtonClicked()
    {
        MenuManager.Instance.OpenMenu("Loading");
        Client.Instance.SendToServer( new NetLoadGame() );
        AudioManager.Instance.PlayButtonClickSfx();
    }

}

