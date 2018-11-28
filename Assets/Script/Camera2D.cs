using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Camera2D : MonoBehaviour {
    private Camera cam;
    public Text DebugText;
    Vector3 NowPos;
    Vector3 DeltaPos;
    Vector3 PrevPos;
    Vector3 angleVec;
    int PrevTouchCount;
    Vector3 PrevCamPos;
    public MineSweeper Mscript;
    private int MaxX;
    private int MaxY;

    void Start()
    {
        cam = GetComponent<Camera>();
        PrevCamPos = cam.transform.position;
        this.MaxX = Mscript.MaxX;
        this.MaxY = Mscript.MaxY;
    }

    /*
    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        //float view = cam.fieldOfView - scroll;

        cam.orthographicSize += scroll;

        if(Input.GetKeyDown("space")){
            if(cam.orthographic == false){
                cam.orthographic = true;
            }
            else{
                cam.orthographic = false;
            }
        }
        //cam.fieldOfView = Mathf.Clamp(value: view, min: 0.1f, max: 45f);
    }
    */

    public float perspectiveZoomSpeed;        // 透視投影モードでの有効視野の変化の速さ
    public float orthoZoomSpeed;        // 平行投影モードでの平行投影サイズの変化の速さ
    public float TouchMoveSpeed;        // 移動の速さ


    void Update()
    {
        //マウス
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        //float view = cam.fieldOfView - scroll;

        cam.orthographicSize += scroll;

        if (Input.GetKeyDown("space"))
        {
            if (cam.orthographic == false)
            {
                cam.orthographic = true;
            }
            else
            {
                cam.orthographic = false;
            }
        }
        //cam.fieldOfView = Mathf.Clamp(value: view, min: 0.1f, max: 45f);


        // 端末に 2 つのタッチがあるならば...
        if (Input.touchCount == 2)
        {
            // 両方のタッチを格納します

            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // 各タッチの前フレームでの位置をもとめます
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // 各フレームのタッチ間のベクター (距離) の大きさをもとめます
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // 各フレーム間の距離の差をもとめます
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // カメラが平行投影ならば...
            if (cam.orthographic)
            {
                // ... タッチ間の距離の変化に基づいて平行投影サイズを変更します
                cam.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;

                // 平行投影サイズが決して 0 未満にならないように気を付けてください
                cam.orthographicSize = Mathf.Max(cam.orthographicSize, 2.0f);
            }
            else
            {
                // そうでない場合は、タッチ間の距離の変化に基づいて有効視野を変更します
                cam.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

                // 有効視野を 0 から 180 の間に固定するように気を付けてください
                cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 0.1f, 179.9f);
            }
        }

        //ボタンでカメラ移動
        cam.transform.Translate(Input.GetAxis("Horizontal")/4,Input.GetAxis("Vertical")/4,0);

        //フリックで画面移動
        // カメラが平行投影ならば...
        if (cam.orthographic)
        {
            this.MaxX = Mscript.MaxX;
            this.MaxY = Mscript.MaxY;
            if (Input.touchCount >= 1)
            {
                if (PrevCamPos == cam.transform.position)
                {

                }
                Vector3 campos = cam.transform.position;
                NowPos = touchPosition();
                foreach (Touch t in Input.touches)
                {
                    if (t.phase == TouchPhase.Began || Input.touchCount != PrevTouchCount)
                    {
                        PrevPos = NowPos;
                    }
                }
                DeltaPos = NowPos - PrevPos;
                campos.x -= DeltaPos.x;
                campos.y -= DeltaPos.y;
                campos.x = Mathf.Clamp(campos.x, 0, MaxX);
                campos.y = Mathf.Clamp(campos.y, 0, MaxY);
                //このスクリプト以外でカメラの位置が変えられた時動かさない
                if (PrevCamPos == cam.transform.position) cam.transform.position = campos;
                PrevPos = touchPosition();
                PrevTouchCount = Input.touchCount;
                PrevCamPos = cam.transform.position;
            }
        }
        else{
            /*
            if (Input.touchCount >= 1)
            {
                angleVec = Input.GetTouch(0).deltaPosition;
                float _a = angleVec.x;
                angleVec.x = angleVec.y;
                angleVec.y = -_a;
                angleVec.z = 0;
                cam.transform.RotateAround(new Vector3((float)MaxX / 2, (float)MaxY / 2, 0), angleVec, 45 * Time.deltaTime);
            }
            else
            {
                angleVec = Vector3.zero;
            }
            */
        }
        //デバッグ用
        //DebugText.text = angleVec.ToString();



        /*
        //デバッグ用
        if (Input.touchCount > 0)
        {
            foreach (Touch t in Input.touches)
            {
                if (t.phase != TouchPhase.Ended && t.phase != TouchPhase.Canceled)
                {

                    Debug.Log("x=" + t.position.x + " y=" + t.position.y);
                }
            }
        }
        */

    }

    public Vector3 touchPosition(){
        Vector3 vec = Vector3.zero;
        for (int i = 0; i < Input.touchCount; i++)
        {
            vec += Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);
        }
        vec /= Input.touchCount;
        return vec;
    }

}
