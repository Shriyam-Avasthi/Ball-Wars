using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InGameUIScreen : Menu
{
    [SerializeField] private VisualTreeAsset playerListItem;
    private PlayerScoreListController playerScoreListController;

    public static InGameUIScreen Instance;

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

        playerScoreListController = new PlayerScoreListController();
        playerScoreListController.InitializePlayerScoreList( root , playerListItem );

    }

    public void RefreshPlayerScoreList()
    {
        if( playerScoreListController != null )
        {
            playerScoreListController.RefreshPlayerScoreList();
        }
    }

}

