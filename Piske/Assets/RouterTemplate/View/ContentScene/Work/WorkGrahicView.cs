using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WorkGrahicView : MonoBehaviour {

    [SerializeField]
    private Button _backButton;

    public System.Action OnBack;

    private void Awake(){
        _backButton.onClick.AddListener (()=>{
            OnBack.Invoke ();
        });
    }

    public void Show(){
        gameObject.SetActive (true);
    }

    public void Hide(){
        gameObject.SetActive (false);
    }
}
