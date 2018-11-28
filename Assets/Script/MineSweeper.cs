using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Effects;
using System;
using System.IO;

public class MineSweeper : MonoBehaviour
{
    public GameObject cell;
    public int MaxX;
    public int MaxY;
    public GameObject[,] array = new GameObject[9, 16];
    public bool[,] IsOpend = new bool[9, 16];
    public bool[,] IsMine = new bool[9, 16];
    public bool[,] IsFlag = new bool[9, 16];
    public int[,] AroundMineNum = new int[9, 16];
    public GameObject obj;
    Cell script;
    public int MineNum;
    public int NowMineNum;
    public int OpendNum = 0;
    public int SafetyNum;
    public bool FirstClick;
    public bool GameOverFlag;
    public bool GameClearFlag;
    public GameObject RetryButton;
    List<Vector2> list = new List<Vector2>();
    List<Vector2> MineSerect = new List<Vector2>();
    List<GameObject> DeleteObject=new List<GameObject>();
    public GameObject Explosion;
    public Camera cam;
    public AudioSource sound01;
    public int Count=0;
    public bool IsCancel;
    public int PutFlagTime;
    public bool IsPutFlag;

    //テキスト関連
    public Text StatusText;
    public Text TimerText;
    public Text MineNumText;
    public Text InfoText;
    public GameObject UI;
    public Text resultText;

    //タイマー関連
    public int TimeCount = 0;
    public bool IsTimerStart;
    public string MilliSecond;
    public string Second;

    //セーブデータ
    public Text DebugText;
    public string GameMode="custom";
    public string[] Ranking;
    List<int> RList = new List<int>();

    //スマホのタッチ用
    public Touch t;
    public GameObject TouchEffect;

    Vector3 PrevScreenPos;


    // Use this for initialization
    void Start()
    {
        //fps固定
        Application.targetFrameRate = 60;
        //カメラ
        cam.orthographic = true;

        Initiate();

        PrevScreenPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //PlayerPrefs.DeleteAll();

        RList = LoadRaning("easy_ranking");
    }

    // Update is called once per frame
    void Update()
    {
        //マウス座標
        Vector3 NowScreenPos = Input.mousePosition;
        Vector3 screenPos = Camera.main.ScreenToWorldPoint(NowScreenPos);
        screenPos.z = 0;
        Vector2 DeltaMousePosition=new Vector2(NowScreenPos.x - PrevScreenPos.x,NowScreenPos.y - PrevScreenPos.y);
        PrevScreenPos =Input.mousePosition;

        //クリック
        //座標
        if(Input.GetMouseButtonDown(0)){
            IsCancel = false;
            Count = 0;
            //スマホに対応
            DeltaMousePosition = Vector2.zero;
            //ハタタテを無駄にしない
            if (IsOpend[(int)screenPos.x, (int)screenPos.y] == false){
                IsPutFlag = true;
            }
            else{
                IsPutFlag = false;
            }
        }

        if (screenPos.x< 0 || screenPos.x >= MaxX || screenPos.y < 0 || screenPos.y >= MaxY){
            IsCancel = true;
        }

        if (Input.GetMouseButton(0))
        {
            if (Input.touchCount >= 1)
            {//微妙？
                if (DeltaMousePosition != Vector2.zero)
                {
                    IsCancel = true;
                }
            }
            if (IsPutFlag == true)
            {
                if (Count == PutFlagTime && IsCancel == false)
                {
                    PutFlag((int)screenPos.x, (int)screenPos.y);
                    IsCancel = true;
                }
                Count++;

                if (IsCancel == false)
                {
                    float t = (float)Count / PutFlagTime;
                    TouchEffect.transform.position = new Vector3((int)screenPos.x + 0.5f, (int)screenPos.y + 0.5f, 0);
                    TouchEffect.GetComponent<Image>().fillAmount = t * t * (3 - 2 * t);
                    TouchEffect.GetComponent<Image>().color = new Color(1, 1, 1, t * t * t);
                }
                else
                {
                    TouchEffect.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                }
            }

            
        }
        else{
            TouchEffect.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }

        if (Input.GetMouseButtonUp(0) && IsCancel == false){
            if (FirstClick == false)
            {
                    MakeMine((int)screenPos.x, (int)screenPos.y, MineNum);
                    OpenContinuously((int)screenPos.x, (int)screenPos.y);
                    IsTimerStart = true;
                    FirstClick = true;

            }
            else
            {
                //GameObject prefab = Instantiate(cell, screenPos, Quaternion.identity);
                if (OpenCell((int)screenPos.x, (int)screenPos.y) == true ||
                    ((AroundMineNum[(int)screenPos.x, (int)screenPos.y] - SearchAroundFlag((int)screenPos.x, (int)screenPos.y)) == 0))
                {
                    OpenContinuously((int)screenPos.x, (int)screenPos.y);
                }
                //Destroy(array[(int)screenPos.x,(int)screenPos.y]);
            }
        }

        if(Input.touchCount>=2){
            IsCancel = true;   
        }
        else{
            if(Input.GetMouseButtonDown(1)){
                PutFlag((int)screenPos.x, (int)screenPos.y);
                Instantiate(TouchEffect,screenPos,Quaternion.identity);
            }
        }

        //デバッグ用
        //text.text = DeltaMousePosition.ToString();
        if(Input.GetKeyDown("space")){
            SaveRanking(GameMode,RList);
            //textSave("qwsedrftgyhujikol");
            Debug.Log("asdfg");
        }
        ShowText();
        Timer();
    }

    // 引数でStringを渡してやる
    public void textSave(string txt)
    {
        StreamWriter sw = new StreamWriter("LogData.txt", false); //true=追記 false=上書き
        sw.WriteLine(txt);
        sw.Flush();
        sw.Close();
    }


    //テキスト表示部分
    public void ShowText(){
        TimerText.text = Second.PadLeft(2, '0');

        if (GameOverFlag == true)
        {
            StatusText.text = "Game Over";
            UI.SetActive(false);
            TouchEffect.SetActive(false);
        }
        else if(GameClearFlag==true){
            
        }
        else
        {
            StatusText.text="";
            MineNumText.text = NowMineNum.ToString().PadLeft(2,'0');
        }

        //デバッグ
        //DebugText.text = Input.mousePosition.ToString();
    }

    //タイマー関連
    public void Timer(){
        if (IsTimerStart == true)TimeCount++;
            Second = (TimeCount / 60).ToString();
        MilliSecond = ((TimeCount * 100 / 60)%100).ToString("d2");
    }

    //(x,y)周囲の地雷の数を数えてそれを返す
    public int SearchMine(int x, int y)
    {
        int Num = 0;
        for (int j = -1; j <= 1; j++)
        {
            for (int i = -1; i <= 1; i++)
            {
                int _x = x + i;
                int _y = y + j;
                if (_x >= 0 && _x < MaxX && _y >= 0 && _y < MaxY)
                {
                    if (IsMine[_x, _y] == true) Num += 1;
                }
            }
        }
        return Num;
    }

    //マスを開ける
    //０のマスを開けたらtrueを返す
    public bool OpenCell(int x,int y){
        if (x >= 0 && x < MaxX && y >= 0 && y < MaxY){
            if (IsOpend[x, y] == false && IsFlag[x, y] == false)
            {
                if (IsMine[x, y] == false)
                {
                    IsOpend[x, y] = true;
                    obj = array[x, y];
                    script = obj.GetComponent<Cell>();
                    script.GetComponent<MeshRenderer>().material = script.materials[1];

                    OpendNum += 1;
                    if (OpendNum == SafetyNum)
                    {//ゲームクリア
                        GameClearFlag = true;
                        StatusText.text = "Game Clear!!";
                        resultText.text = Second + "." + MilliSecond;
                        RetryButton.SetActive(true);
                        IsTimerStart = false;
                        //ランキングに保存
                        SaveRanking(GameMode, RList);
                    }
                    if (AroundMineNum[x, y] == 0) return true;
                    else script.text.GetComponent<TextMesh>().text = AroundMineNum[x, y].ToString();
                }
                else//爆発!!
                {
                    IsOpend[x, y] = true;
                    /*
                    obj = array[x, y];
                    script = obj.GetComponent<Cell>();
                    script.text.GetComponent<TextMesh>().text = "★";
                    */
                    Destroy(array[x, y]);
                    Vector3 placePosition = new Vector3((float)((x + 0.5)), (float)((y + 0.5)), 1.0f);
                    Explosion.GetComponent<ExplosionPhysicsForce>().explosionForce = MaxX * MaxY / 12.6f;
                    obj = Instantiate(Explosion, placePosition, Quaternion.identity);
                    Destroy(obj, 3);
                    GameOverFlag = true;
                    RetryButton.SetActive(true);
                    cam.orthographic = false;
                    cam.transform.position = new Vector3((float)MaxX / 2, (float)MaxY / 2, -20);
                    cam.fieldOfView = 120;
                    sound01.PlayOneShot(sound01.clip);
                    Handheld.Vibrate();//振動
                }
            }   
        }
        return false;
    }

    //周囲のマスを開ける
    public void OpenAroundCell(int x,int y){
        for (int j = -1; j <= 1; j++)
        {
            for (int i = -1; i <= 1; i++)
            {
                int _x = (int)x + i;
                int _y = (int)y + j;
                if (_x >= 0 && _x < MaxX && _y >= 0 && _y < MaxY)
                {
                    if (OpenCell(_x, _y) == true){
                        list.Add(new Vector2(_x, _y));
                    }
                }
            }
        }
    }

    //0なら連続して周囲を開ける
    public void OpenContinuously(int x,int y){
        list.Add(new Vector2(x, y));
        while (list.Count > 0)
        {
            OpenAroundCell((int)list[0].x, (int)list[0].y);
            list.RemoveAt(0);
        }
    }

    //旗を立てる
    public void PutFlag(int x,int y){
        if (IsOpend[x,y] == false){
            if (IsFlag[x, y] == false){
                IsFlag[x, y] = true;
                obj = array[x, y];
                script = obj.GetComponent<Cell>();
                script.text.SetActive(true);
                //script.text.GetComponent<TextMesh>().text = "旗";
                script.Flag.SetActive(true);
                StartCoroutine(FlagAnim(script.Flag));
                NowMineNum -= 1;
            }
            else{
                IsFlag[x, y] = false;
                obj = array[x, y];
                script = obj.GetComponent<Cell>();
                //script.text.GetComponent<TextMesh>().text = "";
                script.Flag.SetActive(false);
                NowMineNum += 1;
            }
        }
    }

    //旗を数えて返す
    public int SearchAroundFlag(int x, int y)
    {
        int FlagNum = 0;
        for (int j = -1; j <= 1; j++)
        {
            for (int i = -1; i <= 1; i++)
            {
                int _x = x + i;
                int _y = y + j;
                if (_x >= 0 && _x < MaxX && _y >= 0 && _y < MaxY)
                {
                    if (IsFlag[_x, _y] == true) FlagNum += 1;
                }
            }
        }
        return FlagNum;
    }

    //初期化
    public void Initiate(){
        //ボタン非表示
        RetryButton.SetActive(false);
        resultText.text = "";

        //UI表示
        UI.SetActive(true);
        TouchEffect.SetActive(true);

        //タイマー初期化
        TimeCount = 0;
        IsTimerStart = false;

        //インフォ
        InfoText.text = MaxX + "×" + MaxY + "\n"+"m "+MineNum;

        //バグ回避用
        IsCancel = true;

        //変数の初期化
        cam.orthographic = true;
        Transform campos = cam.GetComponent<Transform>();
        campos.position = new Vector3((float)MaxX / 2, (float)MaxY / 2+1, -8.0f);
        cam.GetComponent<Camera>().orthographicSize = (float)(MaxX)*8/9;
        NowMineNum = MineNum;
        OpendNum = 0;
        SafetyNum = 0;
        FirstClick = false;
        GameOverFlag = false;
        GameClearFlag = false;

        //配列の初期化
        array = new GameObject[MaxX, MaxY];
        AroundMineNum = new int[MaxX, MaxY];
        IsOpend = new bool[MaxX, MaxY];
        IsMine = new bool[MaxX, MaxY];
        IsFlag = new bool[MaxX, MaxY];


        //オブジェクトを消去
        while(DeleteObject.Count>0){
            Destroy(DeleteObject[0]);
            DeleteObject.RemoveAt(0);
        }

        for (int y = 0; y < MaxY; y++)
        {
            for (int x = 0; x < MaxX; x++)
            {
                Vector3 placePosition = new Vector3((float)(x + 0.5), (float)(y + 0.5), 0);
                GameObject prefab = (GameObject)Instantiate(cell, placePosition, Quaternion.identity);
                array[x, y] = prefab;
                DeleteObject.Add(prefab);
            }
        }
        SafetyNum = (MaxX * MaxY) - MineNum;
    }

    //地雷を作る
    //(x,y):初手の座標
    //num:地雷の数
    public void MakeMine(int x,int y ,int num){
        //初期化
        MineSerect.Clear();
        MineNum = 0;
        //全マスの座標をvector2で入れる
        for (int j = 0; j < MaxY;j++){
            for (int i = 0; i < MaxX;i++){
                MineSerect.Add(new Vector2(i,j));
            }
        }
        //初手とその周囲９マスの座標を抜く
        for (int j = -1; j <= 1; j++)
        {
            for (int i = -1; i <= 1; i++)
            {
                int _x = x + i;
                int _y = y + j;
                if (_x >= 0 && _x < MaxX && _y >= 0 && _y < MaxY)
                {
                    MineSerect.Remove(new Vector2(_x,_y));
                }
            }
        }
        //ランダムに選ぶ
        for (int i = 0; i < num;i++){
            if(MineSerect.Count==0){
                break;
            }
            Vector2 pos=MineSerect[UnityEngine.Random.Range(0,MineSerect.Count)];
            MineSerect.Remove(pos);
            IsMine[(int)pos.x, (int)pos.y]=true;
            array[(int)pos.x, (int)pos.y].GetComponent<Cell>().IsMine = true;
            MineNum += 1;
            //array[(int)pos.x, (int)pos.y].GetComponent<Cell>().text.GetComponent<TextMesh>().text = "★";
        }
        //周りの地雷の数を調べる
        for (int j = 0; j < MaxY; j++)
        {
            for (int i = 0; i < MaxX; i++)
            {
                if (IsMine[i, j] == false)
                {
                    AroundMineNum[i, j] = SearchMine(i, j);
                }
            }
        }
        //解除するマスの総数
        SafetyNum = (MaxX * MaxY) - num;
    }

    //セーブデータ読み込み
    public List<int> LoadRaning(string key){
        string str="";
        string[] strArrary;
        List<int> l = new List<int>();
        if (PlayerPrefs.HasKey(key))
        {
            str = PlayerPrefs.GetString(key);
        }
        else
        {
            PlayerPrefs.SetString(key, "-1\n-1\n-1\n-1\n-1");
            str = PlayerPrefs.GetString(key);
        }
        strArrary=str.Split('\n');
        for (int i = 0; i < strArrary.Length;i++){
            l.Add(int.Parse(strArrary[i]));
        }
        DebugText.text = strArrary.ToString();
        return l;
    }

    //セーブデータ書き込み
    public void SaveRanking(string key,List<int> list){
        for (int i = 0; i < 5; i++)
        {
            if (TimeCount <= RList[i] || RList[i] == -1)
            {
                RList.Insert(i, TimeCount);
                Debug.Log("挿入");
                break;
            }
        }
        string str="";
        for (int i= 0; i < 5;i++){
            str += RList[i].ToString() + '\n';
        }
        PlayerPrefs.SetString("key",str);
        //デバッグ
        str = "";
        for (int i = 0; i < 5; i++)
        {
            if(RList[i]==-1){
                str += (i + 1).ToString() + ":" + "--.--" + '\n';
            }
            else{
                str += (i + 1).ToString() + ":" + RList[i].ToString() + '\n';   
            }
        }
        DebugText.text = str;
        Debug.Log("save");
    }

    private IEnumerator FlagAnim(GameObject obj)
    {
        for (int i = 0; i <= 6; i++)
        {
            float t = i / 6.0f;
            float delta = cam.orthographicSize * 5 / 8;
            obj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f)*(delta-t*t*(delta-1));
            Debug.Log(t);
            yield return null;
        }
    }
}





//Canvasの子オブジェクトを生成する方法
/*
 * public GameObject canvas;
    public GameObject cell;

void Start(){
                Vector3 placePosition = new Vector3(x * 1080 / MaxX-ScreenWidth/2, y * 1080 / MaxY-ScreenHeight/2, 0);
                GameObject prefab=(GameObject)Instantiate(cell, placePosition, Quaternion.identity);
                prefab.transform.SetParent(canvas.transform, false);

 */