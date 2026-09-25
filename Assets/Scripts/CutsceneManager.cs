using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] PlayableDirector timeline;
    [SerializeField] GameObject realPlayer;
    [SerializeField] GameObject dummyPlayer;
    public SignalAsset timelineEnd;


    void Start()
    {
        realPlayer.GetComponent<JoueurMouvement>().peutBouger = false;
        var receiver = GetComponent<SignalReceiver>();
        var unityEvent = new UnityEvent();
        unityEvent.AddListener(OnTimelineFinished);

    }
    public void OnTimelineFinished()
    {
        timeline.Stop();
        realPlayer.SetActive(true);
        dummyPlayer.SetActive(false);
        realPlayer.GetComponent<JoueurMouvement>().peutBouger = true;
    }
}