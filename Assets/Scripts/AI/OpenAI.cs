using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class OpenAI : MonoBehaviour
{

    class RequestMicrophoneData
    {
        public string text_message;
        public string vendor;
    }

    class RequestInitializeVendor
    {
        public string vendor;
        public string item;
    }

    class ResponseVendor
    {
        public string text_message;
        public int price_offer; 
    }

    public class ErrorResponse
    {
        public string error;
    }

    string Base = "";

    public static bool validResponse = false;
    public static string text = "";
    public static int priceOffer = 0;

    private string vendorName;

    public void StartConversation(string name, string item)
    {
        string url = Base + "/";

        RequestInitializeVendor data = new RequestInitializeVendor();
        data.vendor = name;
        data.item = item;

        vendorName = name;

        string jsonData = JsonUtility.ToJson(data);
        byte[] byteData = System.Text.Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(byteData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        StartCoroutine(receive(request));

    }

    public void GenerateResponse(string prompt)
    {
        if (!string.IsNullOrEmpty(prompt))
        {
            SoundAsync(prompt);
        }
    }

    void SoundAsync(string prompt)
    {
        string url = Base + "/receive_input/";
        RequestMicrophoneData data = new RequestMicrophoneData();

        data.text_message = prompt;
        if(vendorName != null)
            data.vendor = vendorName;

        string jsonData = JsonUtility.ToJson(data);
        byte[] byteData = System.Text.Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(byteData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        StartCoroutine(receive(request));

    }

    IEnumerator receive(UnityWebRequest request)
    {
        using (request)
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                CatchError(request);
                validResponse = false;
                
            }
            else
            {
                string response = request.downloadHandler.text;
                ResponseVendor responseData = JsonUtility.FromJson<ResponseVendor>(response);

                text = responseData.text_message;
                priceOffer = responseData.price_offer;

                Debug.Log(text);
                Debug.Log(priceOffer);
                validResponse = true;
            }
        }
    }

    void CatchError(UnityWebRequest request)
    {
        if (request.responseCode == 400)
        {
            string response = request.downloadHandler.text;
            ErrorResponse errorResponse = JsonUtility.FromJson<ErrorResponse>(response);
            Debug.LogError("Server Error: " + errorResponse.error);
        }
        else
        {
            Debug.Log("Error: " + request.error);
        }
    }
}
