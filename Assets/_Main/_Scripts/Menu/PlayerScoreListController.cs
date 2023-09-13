using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class PlayerScoreListController
{
    VisualTreeAsset listItemTemplate;
    ListView playerScoreList;

    public void InitializePlayerScoreList( VisualElement root , VisualTreeAsset _listItemTemplate)
    {
        listItemTemplate = _listItemTemplate;
        playerScoreList = root.Q<ListView>("PlayerScoreListView");

        FillPlayerScoreList();
    }

    private void FillPlayerScoreList()
    {
        
        Func<VisualElement> makeItem = () =>
        {

            var newPlayerListItem = listItemTemplate.Instantiate();
            var newListItemLogic = new PlayerScoreListItem();

            newPlayerListItem.userData = newListItemLogic;
            newListItemLogic.SetVisualElement(newPlayerListItem);
            return newPlayerListItem;

        };
        
        Action<VisualElement, int> bindItem = ( item , index ) =>
        {
            (item.userData as PlayerScoreListItem).SetPlayerData(MenuManager.Instance.playerInfoList[index] );
        };

        playerScoreList.makeItem = makeItem;

        playerScoreList.bindItem = bindItem;

        playerScoreList.fixedItemHeight = 20;
        playerScoreList.itemsSource = MenuManager.Instance.playerInfoList;
    }

    public void RefreshPlayerScoreList()
    {
        playerScoreList.Rebuild();
    }
}
