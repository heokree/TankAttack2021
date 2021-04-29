using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    private new AudioSource audio;
    public AudioClip KaBoomSfx;
    public GameObject KaBoomVfx;

    public string shooter;

    public float cannonForce = 2000.0f;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * cannonForce);
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject obj = Instantiate(KaBoomVfx, collision.GetContact(0).point, Quaternion.identity);
        audio?.PlayOneShot(KaBoomSfx);
        
        Destroy(obj, 3.0f);
        Destroy(this.gameObject, 3.0f);
    }
}
