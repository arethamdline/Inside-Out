using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;

public class OpenAI : MonoBehaviour
{
    [System.Serializable]
    class RequestData
    {
        public string text_message;
        public string character;
    }

    [System.Serializable]
    class ResponseData
    {
        public string text_message;
    }

    [System.Serializable]
    public class ErrorResponse
    {
        public string detail;
    }

    string BaseURL = "http://127.0.0.1:8000";

    public static bool validResponse = false;
    public static string text = "";
    public TextMeshProUGUI characterResponse;

    private string characterName;

    void Start()
    {
        characterName = SceneManager.GetActiveScene().name;
        Debug.Log("conversing with: " + characterName);
    }


    public void GenerateResponse(string prompt)
    {
        if (!string.IsNullOrEmpty(prompt))
        {
            StartCoroutine(SendRequest(prompt));
        }
    }

    IEnumerator SendRequest(string prompt)
    {
        string url = BaseURL + "/receive_input/";

        RequestData data = new RequestData
        {
            text_message = prompt,
            character = characterName
        };

        string jsonData = JsonUtility.ToJson(data);
        byte[] byteData = System.Text.Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(byteData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("sending request: " + prompt);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            HandleError(request);
            validResponse = false;
        }
        else
        {
            string response = request.downloadHandler.text;
            ResponseData responseData = JsonUtility.FromJson<ResponseData>(response);

            text = responseData.text_message;

            Debug.Log("AI Response: " + text);
            characterResponse.text = text;
            validResponse = true;
        }
    }

    void HandleError(UnityWebRequest request)
    {
        string response = request.downloadHandler.text;

        try
        {
            ErrorResponse error = JsonUtility.FromJson<ErrorResponse>(response);
            Debug.LogError("Server Error: " + error.detail);
        }
        catch
        {
            Debug.LogError("Request Error: " + request.error);
        }
    }
}