using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoCameraMover : MonoBehaviour {
    public GameObject goCamera;
    private Vector3 defaultPos;
    public MineSweeper mineSweeper;

    private void Awake()
    {
        // 【Unite Tokyo 2018】Oculusで作るスタンドアローン・モバイルVRコンテンツ
        // https://www.slideshare.net/UnityTechnologiesJapan002/unite-tokyo-2018oculusvr-96453609/UnityTechnologiesJapan002/unite-tokyo-2018oculusvr-96453609
        // 上記スライドを参考にした設定

        // アイバッファ解像度 scale 1.0 == 1024x1024
        UnityEngine.XR.XRSettings.eyeTextureResolutionScale = 1.25f;

        // 外枠側のレンダリング設定。>>HIGH　外側の解像度が減っていく
        OVRManager.tiledMultiResLevel = OVRManager.TiledMultiResLevel.LMSLow;

        // 72Hzモード（フレームレートは上がるが綺麗）
        OVRManager.display.displayFrequency = 72.0f;
    }

    // Use this for initialization
    void Start () {
        defaultPos = goCamera.transform.position;
	}
	
	// Update is called once per frame
	void Update () {    

        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
        {
            ////トリガー押されてたら

        }

        if (OVRInput.GetDown(OVRInput.Button.Back))
        {
            ///戻るボタン押されたら
            goCamera.transform.position = defaultPos;
            mineSweeper.Initiate();
        }

        Vector2 pt = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad); ///タッチパッドの位置
        if (OVRInput.Get(OVRInput.Button.PrimaryTouchpad))
        {
            goCamera.transform.Translate(0, 0, pt.y * Time.deltaTime);
            goCamera.transform.Rotate(0, 0, pt.x * Time.deltaTime);
        }
        else{
            goCamera.transform.Translate(pt.x * Time.deltaTime, pt.y * Time.deltaTime, 0);
        }
        if (OVRInput.GetDown(OVRInput.Touch.PrimaryTouchpad))
        {
            ///タッチパッド触れられたら

        }
               
    }
}
