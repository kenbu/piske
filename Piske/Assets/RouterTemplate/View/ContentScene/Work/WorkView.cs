using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WorkView : MonoBehaviour {

    [SerializeField]
    private Button _graphicButton;

    [SerializeField]
    private Button _webButton;

    public System.Action<string> OnGoto;

    private void Awake(){
        _graphicButton.onClick.AddListener (()=>{
            OnGoto.Invoke ("Graphic");
        });
        _webButton.onClick.AddListener (()=>{
            OnGoto.Invoke ("Web");
        });
    }


    public void Show(){
        gameObject.SetActive (true);
    }

    public void Hide(){
        gameObject.SetActive (false);
    }

}
