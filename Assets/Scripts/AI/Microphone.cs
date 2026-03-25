using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
//using UnityEngine.InputSystem;
using UnityEngine.UI;
using Whisper;
using Whisper.Utils;

public class Microphone : MonoBehaviour
{
    //public InputActionReference gripInputActionReference;
    public OpenAI openAI;

    public WhisperManager whisper;
    public MicrophoneRecord microphoneRecord;

    /*public TextMeshProUGUI tmptext;*/
    string outputText;

    private void Awake()
    {
        microphoneRecord.OnRecordStop += OnRecordStop;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            if (!microphoneRecord.IsRecording)
            {
                microphoneRecord.StartRecord();
            }
            else
            {
                microphoneRecord.StopRecord();
            }
        }

    }

    private async void OnRecordStop(AudioChunk recordedAudio)
    {

        var res = await whisper.GetTextAsync(recordedAudio.Data, recordedAudio.Frequency, recordedAudio.Channels);
        if (res == null || outputText == "")
            return;

        var text = res.Result;

        outputText = text;
        /*tmptext.text = text;*/
        openAI.GenerateResponse(outputText);
    }

}
