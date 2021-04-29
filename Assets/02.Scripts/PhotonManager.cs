using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

// MonoBehaviour를 상속받으면서 Photon 관련 콜백함수들을 모아둔 클래스를 상속받아야함.
public class PhotonManager : MonoBehaviourPunCallbacks
{
    // 게임 버전이 일치하는 사람들끼리 플레이가능하도록 설정함.
    private readonly string gameVersion = "v1.0";
    private string userId = "HOoROo";

    public TMP_InputField userIdText;
    public TMP_InputField roomNameText;

    // 룸목록 저장하기 위한 딕셔너리 자료형 사용
    private Dictionary<string, GameObject> roomDict = new Dictionary<string, GameObject>();
    // 룸을 표시할 prefab
    public GameObject roomPrefab;
    // Room 프리팹이 차일드화 될 부모 객체 (스크롤 뷰의 컨텐츠)
    public Transform scrollCentent;

    void Awake()
    {
        //
        PhotonNetwork.AutomaticallySyncScene = true;

        // 게임 버전 지정
        PhotonNetwork.GameVersion = gameVersion;
        // 유저명 지정
        // PhotonNetwork.NickName = userId;

        //서버 접속 ping test
        PhotonNetwork.ConnectUsingSettings();
    }

    void Start()
    {
        userId = PlayerPrefs.GetString("USER_ID", $"USER_{Random.Range(0, 100):00}");
        userIdText.text = userId;
        PhotonNetwork.NickName = userId;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Server!!");
        // PhotonNetwork.JoinRandomRoom(); // 랜덤한 룸에 접속 시도
        
        // 로비에 접속
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비 입장");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"code={returnCode}, msg={message}");

        // 룸 속성을 설정
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = 30;

        if (string.IsNullOrEmpty(roomNameText.text))
        {
            roomNameText.text = $"ROOM_{Random.Range(0,100):000}";
        }

        // 룸을 생성
        PhotonNetwork.CreateRoom(roomNameText.text, ro);
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

        if (PhotonNetwork.IsMasterClient)
        {
            // 방장이 씬 전환을 하면 모든 구성원이 씬을 전환시키는 메소드
            PhotonNetwork.LoadLevel("BattleField");
        }

        // 통신이 가능한 주인공 캐릭터(탱크) 생성
        // PhotonNetwork.Instantiate("Tank", new Vector3(0, 5.0f, 0), Quaternion.identity, 0);
    }

    // 룸 목록이 변경(갱신) 될때마다 호출되는 콜백함수
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        GameObject tempRoom = null;

        foreach (var room in roomList)
        {
            //룸이 삭제된 경우 
            if (room.RemovedFromList == true)
            {
                // 딕셔너리 삭제, roomItem (Clone) 프리팹 삭제
                roomDict.TryGetValue(room.Name, out tempRoom);
                // 삭제된 방에 대한 RoomItem clone 프리팹 삭제
                Destroy(tempRoom);
                // 딕셔너리에서 데이터를 삭제
                roomDict.Remove(room.Name);
            }
            else // 룸정보가 갱신(변경)
            {
                // 처음 생성된 경우, 딕셔너리에 데이터 추가, roomItem 생성
                if (roomDict.ContainsKey(room.Name) == false)
                {
                    GameObject _room = Instantiate(roomPrefab, scrollCentent);
                    // 룸 정보 표시
                    _room.GetComponent<RoomData>().RoomInfo = room;
                    // 딕셔너리에 데이터 추가
                    roomDict.Add(room.Name, _room);
                }
                else
                {
                    // 룸 정보를 갱신
                    roomDict.TryGetValue(room.Name, out tempRoom);
                    tempRoom.GetComponent<RoomData>().RoomInfo = room;
                }

            }
        }
    }

#region UI_BUTTON_CALLBACK
    public void OnLoginClick()
    {
        if (string.IsNullOrEmpty(userIdText.text))
        {
            userId = $"USER_{Random.Range(0, 100):00}";
            userIdText.text = userId;
        }

        PlayerPrefs.SetString("USER_ID", userIdText.text);
        PhotonNetwork.NickName = userIdText.text;
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnMakeRoomClick()
    {
        // 룸 속성을 설정
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = 30;

        if (string.IsNullOrEmpty(roomNameText.text))
        {
            roomNameText.text = $"ROOM_{Random.Range(0,100):000}";
        }

        // 룸을 생성
        PhotonNetwork.CreateRoom(roomNameText.text, ro);
    }
#endregion
}
