using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class UiScript : MonoBehaviour
{
    public static string Name;
    public static string RoomId;
    public static string PlayerId;
    public TMP_InputField nameInput;
    public TMP_InputField roomIdInput;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetInput(){
        try{
            PlayerId = UnityEngine.Random.Range(111111,999999).ToString();
            Name = nameInput.text;
            RoomId = roomIdInput.text;
            StartCoroutine(Upload());
        }
        catch(System.Exception e){
            Debug.Log(e.Message);
        }
    }

    IEnumerator Upload(){
        string jsonStr = JsonConvert.SerializeObject(new {name= Name, playerId= PlayerId, roomId= RoomId});
        Debug.Log(jsonStr);
        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost:4000/newCon", jsonStr,"application/json")){
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
                SceneManager.LoadScene("SampleScene");
            }
        } 
    }
}
