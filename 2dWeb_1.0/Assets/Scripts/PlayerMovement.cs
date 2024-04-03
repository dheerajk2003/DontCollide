using System.Collections;
using System.Collections.Generic;
using WebSocketSharp;
using UnityEngine;
using Newtonsoft.Json;
using System;
// using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using PimDeWitte.UnityMainThreadDispatcher;
using Unity.VisualScripting;

public class enemyType
{
    public float top;
    public float left;
    public int playerId;
    public string type;
    public string name;
    public float rotation;
    public int health;
}

public class PlayerMovement : MonoBehaviour
{
    float Speed = 4;
    public GameObject enemyPrefab;
    public GameObject bulletPrefab;
    float movX, movY;
    Rigidbody2D rb;
    WebSocket ws;
    public Transform FirePoint;
    public Camera cam;
    Vector2 mousePos;
    private bool canDash = true;
    public SpriteRenderer sr;
    public Transform CamFollow;
    void Start()
    {
        try
        {
            rb = GetComponent<Rigidbody2D>();
            rb.position = new Vector2(DataScript.ranPos[UnityEngine.Random.Range(0,3)],DataScript.ranPos[UnityEngine.Random.Range(0,3)]);
            UiScript.PlayerId = UnityEngine.Random.Range(111111,999999);
            ws = new WebSocket("ws://"+UiScript.ServerIp+"?roomId=" + UiScript.RoomId + "&playerId=" + UiScript.PlayerId + "&name=" + UiScript.Name);
            // ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            Debug.Log("connected ");
            ws.OnMessage += OnWebSocketMessageReceived;
            ws.Connect();
            DataScript.ws = ws;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    void Update()
    {
        try
        {
            movX = Input.GetAxisRaw("Horizontal");
            movY = Input.GetAxisRaw("Vertical");

            mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (canDash)
                {
                    Speed = 30;
                    StartCoroutine(DashCoundown());
                    canDash = false;
                }
            }

            SceneManager.sceneUnloaded += (Scene scene) =>
            {
                ws.Close();
                // ws = null;
            };
        }
        catch(Exception e){
            Debug.Log(e.Message);
        }
    }

    void FixedUpdate()
    {
        try
        {
            if (movX != 0 && movY != 0)
            {
                rb.velocity = new Vector2(movX / 1.4f, movY / 1.4f) * Speed;
            }
            else
            {
                rb.velocity = new Vector2(movX, movY) * Speed;
            }
            // Debug.Log(rb.position.x + " " + rb.position.y);

            Vector2 lookDir = mousePos - rb.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = angle;

            if (ws != null && ws.ReadyState == WebSocketState.Open)
            {
                ws.Send(JsonConvert.SerializeObject(new { type = "message", top = rb.position.y, left = rb.position.x, health = DataScript.health }));
            }

            CamFollow.position = new Vector2(rb.position.x, rb.position.y);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    void OnWebSocketMessageReceived(object sender, MessageEventArgs e)
    {
        try
        {
            enemyType eData = JsonConvert.DeserializeObject<enemyType>(e.Data);
            // if(eData.type != "message"){
            //     Debug.Log("not msg " + eData.playerId + " " + UiScript.PlayerId);
            // }
            if (eData.type == "error")
            {
                Debug.Log("Error occured");
                ws.Close();
                SceneManager.LoadScene("LoginScene");
            }

            else if (eData.type == "remove")
            {
                DesEnemy(eData);

            }

            else if (eData.type == "deMe")
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    DataScript.clearAll();
                    System.GC.Collect();
                    SceneManager.LoadScene("GoverScene");
                });
            }

            else if (eData.type == "bullet")
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    Quaternion rotations = Quaternion.Euler(new Vector3(0, 0, eData.rotation));
                    GameObject bul = Instantiate(bulletPrefab, new Vector3(eData.left, eData.top, 0), rotations);
                    Rigidbody2D rbb = bul.GetComponent<Rigidbody2D>();
                    rbb.AddForce(rotations * Vector2.up * 20, ForceMode2D.Impulse);
                });
            }

            else if (eData.type == "health")
            {
                Debug.Log("recived health = " + eData.health);
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    if (UiScript.PlayerId == eData.playerId)
                    {
                        DataScript.health = eData.health;
                    }
                    else if (DataScript.enimies.ContainsKey(eData.playerId))
                    {
                        EnemyScript escpt = DataScript.enimies[eData.playerId].GetComponent<EnemyScript>();
                        escpt.health = eData.health;
                    }
                });
            }

            else
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    if (DataScript.enimies.ContainsKey(eData.playerId))
                    {
                        EnemyScript escript = DataScript.enimies[eData.playerId].GetComponent<EnemyScript>();
                        if (escript != null)
                        {
                            if (escript.top != eData.top) escript.top = eData.top;
                            if (escript.left != eData.left) escript.left = eData.left;
                            if (escript.health != eData.health) escript.health = eData.health;
                            if (escript.ename != eData.name) escript.ename = eData.name;
                        }

                    }
                    else if (!DataScript.removedEnimies.Contains(eData.playerId))
                    {
                        Debug.Log("creating enemy" + eData.playerId);
                        var newEnemy = Instantiate(enemyPrefab, new Vector2(eData.left, eData.top), Quaternion.identity);
                        EnemyScript escript = newEnemy.GetComponent<EnemyScript>();
                        DataScript.enimies.Add(eData.playerId, newEnemy);
                    }
                });
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log("application is getting closed");
        ws.Close();
    }

    void DesEnemy(enemyType eData)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            if (DataScript.enimies.ContainsKey(eData.playerId))
            {
                Debug.Log("destroying enemy" + eData.playerId);
                Destroy(DataScript.enimies[eData.playerId].gameObject);
                DataScript.removedEnimies.Add(eData.playerId);
                DataScript.enimies.Remove(eData.playerId);
            }
            else
            {
                Debug.Log("not found" + eData.playerId);
            }
        });
    }

    IEnumerator DashCoundown()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(.1f);
        Speed = 4;
        StartCoroutine(DashCoolDown());
    }

    IEnumerator DashCoolDown()
    {
        yield return new WaitForSeconds(2f);
        sr.color = Color.green;
        canDash = true;
    }

    
}