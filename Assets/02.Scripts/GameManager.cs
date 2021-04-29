using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
#region Field 선언
    [Header("Room Info")]
    public TMP_Text roomNameText;
    public TMP_Text connectInfoText;
    public TMP_Text messageText;

    [Header("Chatting UI")]
    public TMP_Text chatListText;
    // public TMP_InputField msgIF;
    public TMP_InputField msgInputField;

    private PhotonView pv;

    public Button exitButton;

    // singleton 변수
    public static GameManager instance = null;
#endregion

#region Unity 함수
    void Awake()
    {
        instance = this;

        Vector3 pos = new Vector3(Random.Range(-150f, 150.0f), 5.0f, Random.Range(-150.0f, 150.0f));

        // 통신이 가능한 주인공 캐릭터(탱크) 생성
        PhotonNetwork.Instantiate("Tank", pos, Quaternion.identity, 0);
    }

    void Start()
    {
        pv = GetComponent<PhotonView>();
        SetRoomInfo();
    }
#endregion

#region room 관련 UI 로직
    void SetRoomInfo()
    {
        Room currentRoom = PhotonNetwork.CurrentRoom;
        roomNameText.text = currentRoom.Name;
        connectInfoText.text = $"{currentRoom.PlayerCount}/{currentRoom.MaxPlayers}";
    }

    public void OnExitClick()
    {
        PhotonNetwork.LeaveRoom();
    }

    // CleanUp 끝난후에 호출되는 콜백 - 즉, 룸에서 완전히 빠져나옴
    public override void OnLeftRoom()
    {
        // Lobby 씬으로 되돌아 가기...
        SceneManager.LoadScene("Lobby");
    }

    // 플레이어 숫자 갱신을 위해 정보갱신로직
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        SetRoomInfo();
        string msg = $"\n<color=#00ff00>{newPlayer.NickName}</color> is joined room";
        messageText.text += msg;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        SetRoomInfo();
        string msg = $"\n<color=#ff0000>{otherPlayer.NickName}</color> left room";
        messageText.text += msg;
    }
#endregion

#region chating 구현
    public void OnSendClick()
    {
        string _msg = $"<color=#00ff00>[{PhotonNetwork.NickName}]</color> {msgInputField.text}";
        pv.RPC("SendChatMessage", RpcTarget.AllBufferedViaServer, _msg);
    }

    [PunRPC]
    void SendChatMessage (string msg)
    {
        chatListText.text += $"{msg}\n";
    }
#endregion
}
