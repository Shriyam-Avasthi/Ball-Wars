using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerListItemController
{
    Label m_NameLabel;

    public void SetVisualElement(VisualElement visualElement)
    {
        m_NameLabel = visualElement.Q<Label>("PlayerName");
    }

    public void SetPlayerData(PlayerInfo playerInfo)
    {
        m_NameLabel.text = playerInfo.nickName;
    }
}