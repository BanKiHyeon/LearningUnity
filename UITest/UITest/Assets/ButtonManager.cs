using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonManager : InjectorBehaviour
{
    [DI(DIScope.singleton)] public TestViewModel TestViewModel { get; }
    
    //singleton
    private static ButtonManager _instance = null;

    public static ButtonManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(ButtonManager)) as ButtonManager;

                if (!_instance)
                {
                    var obj = new GameObject("ButtonGroup");
                    _instance = obj.AddComponent<ButtonManager>();
                    DontDestroyOnLoad(obj);
                }
            }

            return _instance;
        }
    }
    //singleton

    [SerializeField] private Button button1;
    [SerializeField] private Button button2;
    [SerializeField] private Button button3;
    [SerializeField] private Button button4;

    [SerializeField] private TMPro.TextMeshProUGUI button1Text;
    [SerializeField] private TMPro.TextMeshProUGUI button2Text;
    [SerializeField] private TMPro.TextMeshProUGUI button3Text;
    [SerializeField] private TMPro.TextMeshProUGUI button4Text;

    private void Awake()
    {
#if UNITY_EDITOR
        //singlton
        if (!_instance)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        //singleton

#elif UNITY_ANDROID
        if (!isStatusUpdate)
        {
            StartCoroutine(ProcessStatusVisible(0.5F));
        }
#endif
    }

    private void OnEnable()
    {
        button1.onClick.RemoveAllListeners();
        button2.onClick.RemoveAllListeners();
        button3.onClick.RemoveAllListeners();
        button4.onClick.RemoveAllListeners();

        button1.onClick.AddListener(() => { OnClickButton("button1"); });
        button2.onClick.AddListener(() => { OnClickButton("button2"); });
        button3.onClick.AddListener(() => { OnClickButton("button3"); });
        button4.onClick.AddListener(() => { OnClickButton("button4"); });
    }

private void OnClickButton(string value)
    {
        Debug.Log($"onClick : {value}");
    }
}