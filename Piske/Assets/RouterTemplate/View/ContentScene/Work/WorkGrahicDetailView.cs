using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WorkGrahicDetailView : MonoBehaviour {

    [SerializeField]
    private Button _closeButton;
    [SerializeField]
    private Text _title;
    [SerializeField]
    private Image _image;

    public System.Action OnClose;

    private void Awake(){
        _closeButton.onClick.AddListener (()=>{
            OnClose.Invoke ();
        });
    }

    public void UpdateDate(){

    }
    public void UpdateDate(string title, string image){
        _title.text = title;
        //_image
    }

    public void Show(){
        gameObject.SetActive (true);
    }

    public void Hide(){
        gameObject.SetActive (false);
    }
}
