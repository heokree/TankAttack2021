using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Utility;

public class TankCtrl : MonoBehaviour
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
    }

    [PunRPC]
    void Fire(string shooterName)
    {
        GameObject _cannon = Instantiate(cannon, firePos.position, firePos.rotation);
        audio?.PlayOneShot(fireCannon);
        _cannon.GetComponent<Cannon>().shooter = shooterName;
    }
}
