using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerListItem : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    public void SetUp(PlayerInfo info )
    {
        text.text = info.nickName;
    }
}
