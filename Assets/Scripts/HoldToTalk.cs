using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whisper;
using Whisper.Utils;
using UnityEngine.EventSystems;

public class HoldToTalk : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public MicrophoneRecord microphoneRecord;

    public void OnPointerDown(PointerEventData eventData)
    {
        microphoneRecord.StartRecord();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        microphoneRecord.StopRecord();
    }
}
