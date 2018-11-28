using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class setting : MonoBehaviour {
    public Slider MineSlider;
    public Slider MaxXSlider;
    public Slider MaxYSlider;
    public Slider PutFlagTimeSlider;
    public int MineNum;
    public int MaxX;
    public int MaxY;
    public int PutFlagTime;
    public bool IsMenu;
    public GameObject Panel;
    public MineSweeper Mscript;
    public Camera2D Cscript;
    public Text ScoreText;
    public Text DebugText;

	// Use this for initialization
	void Start () {
        MineNum = Mscript.MineNum;
        MineSlider.value = MineNum;
        MaxX = Mscript.MaxX;
        MaxXSlider.value = MaxX;
        MaxY = Mscript.MaxY;
        MaxYSlider.value = MaxY;
        PutFlagTime = Mscript.PutFlagTime;
        PutFlagTimeSlider.value = PutFlagTime;
	}
	
	// Update is called once per frame
	void Update () {
        MineNum = (int)MineSlider.value;
        MaxX = (int)MaxXSlider.value;
        MaxY = (int)MaxYSlider.value;
        MineSlider.maxValue = MaxX * MaxY - 10;
        PutFlagTime = (int)PutFlagTimeSlider.value;

        //よくわからん
        /*
        if(GetComponent<RectTransform>().anchoredPosition==new Vector2(0,216))
        {
            if (Input.touchCount >= 1)
            {
                Touch t = Input.GetTouch(0);
                if (t.deltaPosition.x < -10)
                {
                    StartCoroutine(SinShift(new Vector2(-1080, 0)));
                    IsMainPos = false;
                }
                else if (t.deltaPosition.x > 10)
                {
                    StartCoroutine(SinShift(new Vector2(1080, 0)));
                    IsMainPos = false;
                }
                else if (t.deltaPosition.y > 10)
                {
                    StartCoroutine(SinShift(new Vector2(0, 1920)));
                    IsMainPos = false;
                }
            }
        }
        */

	}

    public void OnMenuButton(){
        if (IsMenu == false){
            Panel.SetActive(true);
            Mscript.enabled = false;
            Cscript.enabled = false;
            IsMenu = true;
            ScoreText.text = DebugText.text;
        }
        else{
            Panel.SetActive(false);
            Cscript.enabled = true;
            IsMenu = false;
            Mscript.enabled = true;
            Mscript.PutFlagTime = PutFlagTime;
            if(Mscript.MineNum != this.MineNum || Mscript.MaxX != this.MaxX || Mscript.MaxY != this.MaxY){
                Mscript.MineNum = this.MineNum;
                Mscript.MaxX = this.MaxX;
                Mscript.MaxY = this.MaxY;
                Mscript.GameMode = GameModeCheck();
                Mscript.Initiate();
            }
        }
    }

    public void OnEasyButton(){
        MaxXSlider.value = 9;
        MaxYSlider.value = 9;
        MineSlider.maxValue = 10;
        MineSlider.value = 10;
    }
    public void OnNormalButton()
    {
        MaxXSlider.value = 16;
        MaxYSlider.value = 16;
        MineSlider.maxValue = 40;
        MineSlider.value = 40;
    }
    public void OnHardButton()
    {
        MaxXSlider.value = 16;
        MaxYSlider.value = 30;
        MineSlider.maxValue = 99;
        MineSlider.value = 99;
    }
    public void OnUltraButton()
    {
        MaxXSlider.value = 24;
        MaxYSlider.value = 48;
        MineSlider.maxValue = 256;
        MineSlider.value = 256;
    }
    public void OnManiaButton()
    {
        MaxXSlider.value = 48;
        MaxYSlider.value = 68;
        MineSlider.maxValue = 777;
        MineSlider.value = 777;
    }

    bool IsPossible=true;

    public void OnButtonShiftX(int shiftX)
    {
        StartCoroutine(SinShift(new Vector2(shiftX,0)));
    }

    public void OnButtonShiftY(int shiftY)
    {
        StartCoroutine(SinShift(new Vector2(0, shiftY)));
    }

    private string GameModeCheck(){
        if (MaxX == 9 && MaxY == 9 && MineNum == 10) return "easy";
        else if (MaxX == 16 && MaxY == 16 && MineNum == 40) return "normal";
        else if (MaxX == 16 && MaxY == 30 && MineNum == 99) return "hard";
        else if (MaxX == 24 && MaxY == 48 && MineNum == 256) return "ultra";
        else if (MaxX == 48 && MaxY == 68 && MineNum == 777) return "mania";
        else return "custom";
    }

    //補完用コルーチン  
    private IEnumerator SinShift(Vector2 shiftVec) {
        // コルーチンの処理  
        Vector2 vec = GetComponent<RectTransform>().anchoredPosition;
        if (IsPossible == true)
        {
            IsPossible = false;
            for (int i = 1; i <= 15; i++)
            {
                float t = i / 15f;
                //GetComponent<RectTransform>().anchoredPosition = vec + new Vector2(shiftX * Mathf.Sin(Mathf.PI / 2 * i / 30), 0);
                //GetComponent<RectTransform>().anchoredPosition = vec + new Vector2(shiftX * Mathf.Pow(i / 30f,2), 0);
                GetComponent<RectTransform>().anchoredPosition = vec + shiftVec * t * (2 - t);
                //GetComponent<RectTransform>().anchoredPosition = vec + shiftVec * t * t * (3 - 2 * t);
                //Debug.Log(t);
                yield return null;
            }
            IsPossible = true;
        }
    }  
}
