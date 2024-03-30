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
            if(nameInput.text.Length >= 6 && roomIdInput.text.Length >=6){
                PlayerId = UnityEngine.Random.Range(111111,999999);
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
