using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class PlayerInfoListController
{
    VisualTreeAsset listItemTemplate;
    ListView playerInfoList;

    public void InitializePlayerInfoList( VisualElement root , VisualTreeAsset _listItemTemplate)
    {
        listItemTemplate = _listItemTemplate;
        playerInfoList = root.Q<ListView>("PlayerInfoListView");

        FillPlayerInfoList();
    }
    // List<PlayerInfo> infos;
    private void FillPlayerInfoList()
    {        
        Func<VisualElement> makeItem = () =>
        {

            var newPlayerListItem = listItemTemplate.Instantiate();
            var newListItemLogic = new PlayerListItemController();

            newPlayerListItem.userData = newListItemLogic;
            newListItemLogic.SetVisualElement(newPlayerListItem);
            return newPlayerListItem;

        };
        
        Action<VisualElement, int> bindItem = ( item , index ) =>
        {
            (item.userData as PlayerListItemController).SetPlayerData(MenuManager.Instance.playerInfoList[index] );
        };

        playerInfoList.makeItem = makeItem;

        playerInfoList.bindItem = bindItem;

        playerInfoList.fixedItemHeight = 5;
        playerInfoList.itemsSource = MenuManager.Instance.playerInfoList;
    }

    public void RefreshPlayerInfoList()
    {
        playerInfoList.Rebuild();
    }
}
