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
    public float textSpeed = 0.02f;

    private string characterName;
    private Coroutine typingCoroutine;
    private string fullText = "";

    void Start()
    {
        characterName = SceneManager.GetActiveScene().name;
        Debug.Log("conversing with: " + characterName);

        SetCharacterColor();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                characterResponse.text = fullText;
            }
        }
    }

    public void GenerateResponse(string prompt)
    {
        Debug.Log("GenerateResponse CALLED with: " + prompt);
        
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
            fullText = text;

            Debug.Log("AI Response: " + text);

            DisplayResponse(text);

            validResponse = true;
        }
    }

    void DisplayResponse(string newText)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeText(newText));
    }

    IEnumerator TypeText(string line)
    {
        characterResponse.text = "";

        foreach (char c in line)
        {
            characterResponse.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void SetCharacterColor()
    {
        switch (characterName)
        {
            case "Joy":
                characterResponse.color = Color.yellow;
                break;
            case "Anger":
                characterResponse.color = Color.red;
                break;
            case "Fear":
                characterResponse.color = new Color(0.647f, 0.392f, 0.725f);
                break;
            case "Disgust":
                characterResponse.color = Color.green;
                break;
            case "Sadness":
                characterResponse.color = Color.blue;
                break;
            default:
                characterResponse.color = Color.white;
                break;
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