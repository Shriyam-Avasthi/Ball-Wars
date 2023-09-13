using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Menu[] menus;
    [SerializeField] private Transform lobby_PlayerListHolder;
    [SerializeField] private GameObject prefab_PlayerListItem;
    [SerializeField] private GameObject startGameButton;
    [SerializeField] private GameObject waitScreen;

    public List<PlayerInfo> playerInfoList = new List<PlayerInfo>();
    public List<RoomInfo> roomInfoList = new List<RoomInfo>();

    public static MenuManager Instance;

    private void Awake() {
        if( Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
        // playerInfoList.Add( new PlayerInfo("ABC" , 10 , 0));
        
    }

    private void Start()
    {
        OpenMenu("MainMenu");
    }
    
    public void OpenMenu(string menuName)
    {
        for(int i = 0; i< menus.Length; i++)
        {
            if(menus[i].menuName == menuName)
            {
                menus[i].Open();
            }
            else if(menus[i].isOpen)
            {
                menus[i].Close();
            }
        }
    } 

    public void OpenMenu(Menu menu)
    {
         for(int i = 0; i< menus.Length; i++)
        {
            if(menus[i].isOpen)
            {
                CloseMenu(menus[i]);               
            }
        }
        menu.Open();
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }

    public void CloseCurrentMenu()
    {
        for( int i = 0 ; i < menus.Length ; i++)
        {
            if( menus[i].isOpen)
            {
                menus[i].Close();
            }
        }
    }

    public void UpdateRoomInfoList( List<RoomInfo> infos)
    {
        roomInfoList.Clear();

        for( int i = 0; i < infos.Count ; i++)
        {
            roomInfoList.Add( infos[i]);
        }
        if( JoinRoomMenu.Instance != null)
        JoinRoomMenu.Instance.RefreshRoomInfoList();
    }   

    public void Lobby_UpdatePlayerList(List<PlayerInfo> infos)
    {
        playerInfoList.Clear();

        for( int i = 0 ; i < infos.Count ; i++)
        {
            playerInfoList.Add(infos[i]);
        }
        LobbyMenu.Instance.RefreshPlayerInfoList();
    }

    public void UpdatePlayerScore( int id , int score )
    {
        for( int i = 0; i < playerInfoList.Count; i++)
        {
            if( playerInfoList[i].PlayerID == id )
            {
                playerInfoList[i].score += score;
                Debug.Log("MM" +  score.ToString());
            }
        }
        InGameUIScreen.Instance.RefreshPlayerScoreList();
    }

    public void ActivateMasterMenu()
    {
        LobbyMenu.Instance.startGameButton.SetEnabled(true);
    }

    public void DeactivateWaitScreen()
    {
        waitScreen.SetActive(false);
    }

}