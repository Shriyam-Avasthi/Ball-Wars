using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RoomInfoListItemController
{
    Label m_NameLabel;
    Label m_NumOfPlayers;

    public void SetVisualElement(VisualElement visualElement)
    {
        m_NameLabel = visualElement.Q<Label>("RoomName");
        m_NumOfPlayers = visualElement.Q<Label>("CurrentNoPlayer");
    }

    public void SetPlayerData(RoomInfo roomInfo)
    {
        m_NameLabel.text = roomInfo.name;
        m_NumOfPlayers.text = roomInfo.numOfPlayers.ToString();
    }
}