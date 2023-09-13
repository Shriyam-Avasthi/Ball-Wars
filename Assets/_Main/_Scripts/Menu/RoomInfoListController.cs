using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class RoomInfoListController
{
    private VisualTreeAsset listItemTemplate;
    private ListView roomInfoList;
    private ushort joinRoomPort = 0;

    public void InitializeRoomInfoList( VisualElement root , VisualTreeAsset _listItemTemplate)
    {
        listItemTemplate = _listItemTemplate;
        roomInfoList = root.Q<ListView>("RoomInfoListView");

        JoinRoomMenu.Instance.joinRoomButton.clickable.clicked += OnJoinRoomButtonClicked;
        roomInfoList.onSelectionChange += OnRoomSelected;

        FillRoomInfoList();
    }

    private void FillRoomInfoList()
    {
        Func<VisualElement> makeItem = () =>
        {

            var newRoomListItem = listItemTemplate.Instantiate();
            var newListItemLogic = new RoomInfoListItemController();

            newRoomListItem.userData = newListItemLogic;
            newListItemLogic.SetVisualElement(newRoomListItem);
            return newRoomListItem;

        };
        
        Action<VisualElement, int> bindItem = ( item , index ) =>
        {
            (item.userData as RoomInfoListItemController).SetPlayerData(MenuManager.Instance.roomInfoList[index] );
        };

        roomInfoList.makeItem = makeItem;

        roomInfoList.bindItem = bindItem;

        roomInfoList.fixedItemHeight = 5;
        roomInfoList.itemsSource = MenuManager.Instance.roomInfoList;
    }

    private void OnRoomSelected( IEnumerable<object> selectedItems)
    {
        var selectedRoom = roomInfoList.selectedItem as RoomInfo;
        Debug.Log("Selected");
        if( selectedRoom != null)
        {
            JoinRoomMenu.Instance.joinRoomButton.SetEnabled(true);
            joinRoomPort = selectedRoom.port;
            Debug.Log("Not Null");
        }
        else
        {
            JoinRoomMenu.Instance.joinRoomButton.SetEnabled(false);
            Debug.Log("Null");
        }
    }

    private void OnJoinRoomButtonClicked()
    {
        if( joinRoomPort != 0 )
        {
            MasterClient.Instance.JoinRoom(joinRoomPort);
            MenuManager.Instance.OpenMenu("Loading");
        }
    }

    public void RefreshRoomInfoList()
    {
        roomInfoList.Rebuild();
    }
}
