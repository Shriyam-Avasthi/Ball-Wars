using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerScoreListItem
{
    Label nameLabel;
    Label scoreLabel;

    public void SetVisualElement(VisualElement visualElement)
    {
        nameLabel = visualElement.Q<Label>("PlayerName");
        scoreLabel = visualElement.Q<Label>("ScoreLabel");
    }

    public void SetPlayerData(PlayerInfo playerInfo)
    {
        nameLabel.text = playerInfo.nickName;
        scoreLabel.text = playerInfo.score.ToString();
    }
}