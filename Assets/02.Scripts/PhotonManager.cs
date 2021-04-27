using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

// MonoBehaviour를 상속받으면서 Photon 관련 콜백함수들을 모아둔 클래스를 상속받아야함.
public class PhotonManager : MonoBehaviourPunCallbacks
{
    // 게임 버전이 일치하는 사람들끼리 플레이가능하도록 설정함.
    private readonly string gameVersion = "v1.0";
    private string UserId = "HR";

    void Awake()
    {
        // 게임 버전 지정
        PhotonNetwork.GameVersion = gameVersion;
        // 유저명 지정
        PhotonNetwork.NickName = UserId;

        //서버 접속
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Server!!");
        PhotonNetwork.JoinRandomRoom(); // 랜덤한 룸에 접속 시도
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"code={returnCode}, msg={message}");
        // 룸을 생성
        PhotonNetwork.CreateRoom("My Room");
    }

    // 룸 생성 완료 콜백
    public override void OnCreatedRoom()
    {
        Debug.Log("방 생성 완료");
    }

    // 룸에 입장했을 때 호출되는 콜백함수
    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장 완료");
        Debug.Log(PhotonNetwork.CurrentRoom.Name);
    }
}