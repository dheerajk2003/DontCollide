using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ChangeScn());
    }

    IEnumerator ChangeScn(){
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("SampleScene");
    }
}
