using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UiScript : MonoBehaviour
{
    public static string Name;
    public static string RoomId;
    public static int PlayerId;
    public TMP_InputField nameInput;
    public TMP_InputField roomIdInput;

    void Awake(){
        PlayerId = Random.Range(111111,999999);
    }

    public void GetInput(){
        try{
            if(nameInput.text.Length >= 6 && roomIdInput.text.Length >=6){
                Name = nameInput.text;
                RoomId = roomIdInput.text;
                SceneManager.LoadScene("SampleScene");
            }
            else{

            }
        }
        catch(System.Exception e){
            Debug.Log(e.Message);
        }
    }
}
