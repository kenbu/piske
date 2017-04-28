using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GlobalMenu : MonoBehaviour {

    [SerializeField]
    private Button _historyBackButton;
    [SerializeField]
    private Button _homeButton;

    [SerializeField]
    private Button _workButton;
    [SerializeField]
    private Text _text;


    public System.Action<string> OnGoto;
    public System.Action OnHistoryBack;

	// Use this for initialization
	void Awake () {
        _historyBackButton.onClick.AddListener (()=>{
            OnHistoryBack.Invoke();
        });
        _homeButton.onClick.AddListener (()=>{
            OnGoto.Invoke("/Root/Home");
        });
        _workButton.onClick.AddListener (()=>{
            OnGoto.Invoke("/Root/Work");
        });
	}
	
	// Update is called once per frame
    public void UpdateData (string url) {
        _text.text = url;
	}
}
