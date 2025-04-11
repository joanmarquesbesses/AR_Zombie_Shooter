using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasActivator : MonoBehaviour
{
    #region Singleton
    private static CanvasActivator _instance;

    public static CanvasActivator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CanvasActivator>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("CanvasActivator");
                    _instance = singletonObject.AddComponent<CanvasActivator>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }
    #endregion

    [System.Serializable]
    public class CanvasControl
    {
        public string name;
        public Canvas canvas;
        //public KeyCode activationKey;
        public List<Button> activationButtons = new List<Button>();
        public List<Button> deactivationButtons = new List<Button>();
    }

    public CanvasControl[] canvasControls;
    public Action<bool, string> OnCanvasActivated;

    void Start()
    {
        for (int i = 0; i < canvasControls.Length; i++)
        {
            if (canvasControls[i].activationButtons != null)
            {
                int index = i;
                foreach (Button button in canvasControls[i].activationButtons)
                {
                    button.onClick.AddListener(() => ToggleCanvas(canvasControls[index], true));
                }
            }

            if (canvasControls[i].deactivationButtons != null)
            {
                int index = i;
                foreach (Button button in canvasControls[i].deactivationButtons)
                {
                    button.onClick.AddListener(() => ToggleCanvas(canvasControls[index], false));
                }
            }
        }
    }

    void Update()
    {
        //for (int i = 0; i < canvasControls.Length; i++)
        //{
        //    if (Input.GetKeyDown(canvasControls[i].activationKey))
        //    {
        //        ToggleCanvas(canvasControls[i], !canvasControls[i].canvas.gameObject.activeSelf);
        //    }
        //}
    }

    public void ToggleCanvas(CanvasControl canvasControl, bool state)
    {
        OnCanvasActivated?.Invoke(state, canvasControl.name);
        if (canvasControl != null)
        {
            Debug.Log($"Canvas {canvasControl.name} active: {state}");
            canvasControl.canvas.gameObject.SetActive(state);
        }
        else
        {
            Debug.Log("Canvas Not Assigned");
        }
    }

    public bool IsCanvasActive(string canvasName)
    {
        for (int i = 0; i < canvasControls.Length; i++)
        {
            if (canvasControls[i].name == canvasName && canvasControls[i].canvas.gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsAnyCanvasActive()
    {
        foreach (var canvas in canvasControls)
        {
            if (canvas.canvas.isActiveAndEnabled)
                return true;
        }
        return false;
    }

    public void SetActiveCanvas(string name, bool active)
    {
        foreach(var canvas in canvasControls)
        {
            if(canvas.name == name)
            {
                canvas.canvas.gameObject.SetActive(active);
            }
        }
    }
}