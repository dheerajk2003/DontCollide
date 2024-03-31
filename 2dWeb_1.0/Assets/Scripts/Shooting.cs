using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Transform FirePoint;
    public GameObject BulletPrefab;
    public BoxCollider2D box;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1")){
            Fire();
        }
        
    }

    void Fire(){
        DataScript.sendBullet(FirePoint.position.x,FirePoint.position.y,FirePoint);
        GameObject bullet = Instantiate(BulletPrefab,FirePoint.position, FirePoint.rotation);
        Debug.Log(FirePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(FirePoint.up * DataScript.bulletForce, ForceMode2D.Impulse);
    }
}
