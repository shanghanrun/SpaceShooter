using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Unity-UI
using UnityEngine.Events; // UnityEvent  관련 API 사용하기 위해


public class UIManager : MonoBehaviour
{
    public Button startButton;
    public Button optionButton;
    public Button shopButton;

    private UnityAction action;  // 콜백함수와 유사

    void Start(){
        //UnityAction을 이용한 이벤트 연결방식
        action =()=> OnButtonClick(startButton.name); //startButton의 게임오브젝트이름
        startButton.onClick.AddListener(action);

        //무명 메서드를 활용한 이벤트 연결방식
        optionButton.onClick.AddListener(delegate {OnButtonClick(optionButton.name);});

        //람다식을 활용한 이벤트 연결방식
        shopButton.onClick.AddListener(()=> OnButtonClick(shopButton.name));
    }
    public void OnButtonClick(string msg) {
        print($"Clicked Button: {msg}");
    }
}
