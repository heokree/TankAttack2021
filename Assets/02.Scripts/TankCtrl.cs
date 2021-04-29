using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Utility;

public class TankCtrl : MonoBehaviour, IPunObservable
{
    private Transform tr;
    public float speed = 10.0f;

    private PhotonView pv;

    public Transform firePos;
    public GameObject cannon;

    public Transform cannonMesh;

    private new AudioSource audio;
    public AudioClip fireCannon;

    public TMPro.TMP_Text userIdText;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        pv = GetComponent<PhotonView>();
        audio = GetComponent<AudioSource>();

        userIdText.text = pv.Owner.NickName;

        if (pv.IsMine)
        {
            Camera.main.GetComponent<SmoothFollow>().target = tr.Find("CamPivot").transform;
            GetComponent<Rigidbody>().centerOfMass = new Vector3(0, -5.0f, 0);
        }
        else
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (pv.IsMine)
        {
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");

            tr.Translate(Vector3.forward * Time.deltaTime * speed * v);
            tr.Rotate(Vector3.up * Time.deltaTime * 100.0f * h);

            // 포탄 발사 로직
            if(Input.GetMouseButtonDown(0))
            {
                pv.RPC("Fire", RpcTarget.AllViaServer, pv.Owner.NickName);
            }

            // 포신 회전 설정
            float r = Input.GetAxis("Mouse ScrollWheel");
            cannonMesh.Rotate(Vector3.right* Time.deltaTime * r * 200.0f);
        }
        else
        {   
            // dead reckoning 에 대한 구현
            if ((tr.position - receivePos).sqrMagnitude > 3.0f * 3.0f)
            {
                tr.position = receivePos;
            }
            else
            {
                tr.position = Vector3.Lerp(tr.position, receivePos, Time.deltaTime * 10.0f);
            }
            tr.rotation = Quaternion.Slerp(tr.rotation, receiveRot, Time.deltaTime * 10.0f);
        }
    }

    [PunRPC]
    void Fire(string shooterName)
    {
        GameObject _cannon = Instantiate(cannon, firePos.position, firePos.rotation);
        audio?.PlayOneShot(fireCannon);
        _cannon.GetComponent<Cannon>().shooter = shooterName;
    }

    // 네트워크를 통해서 수신받을 변수
    Vector3 receivePos      = Vector3.zero;
    Quaternion receiveRot   = Quaternion.identity;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // PhotonView IsMine == true
        {
            stream.SendNext(tr.position); // 위치
            stream.SendNext(tr.rotation); // 회전값
        }
        else
        {
            // 위에서 2가지 2번에 걸쳐 보냈으므로 2번 받아야함. 보낸 순서대로 받아야 함
            receivePos = (Vector3) stream.ReceiveNext(); 
            receiveRot = (Quaternion) stream.ReceiveNext();
        }
    }
}
