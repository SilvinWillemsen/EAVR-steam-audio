﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomSizeManager : MonoBehaviour
{
    public GameObject root;
    // public SourceManager sourceManager;
    public GameObject playerGO;
    public PlayerManager playerManager;
    public AudioSourceManager audioSourceManager;
    public GameObject roomsizeGO;
    // public SpeakerWallManager speakerWallManager;

    private TMP_InputField xText, yText, zText;
    private float prevXval, prevYval, prevZval;
    private Slider xSlider, ySlider, zSlider;
    private float playerX = 0.5f;
    private float playerZ = 0.5f;

    private List<Vector3> sourcePositions = new List<Vector3>();

    public float curRoomWidth, curRoomHeight, curRoomDepth;

    public Vector3 originalSize;

    Bounds getRenderBounds(GameObject objeto){
        Bounds bounds = new  Bounds(Vector3.zero,Vector3.zero);
        Renderer render = objeto.GetComponent<Renderer>();
        if(render!=null){
            return render.bounds;
        }
        return bounds;
    }

    public Bounds getBounds(GameObject objeto){
        Bounds bounds;
        Renderer childRender;
        bounds = getRenderBounds(objeto);
        if(bounds.extents.x == 0){
            bounds = new Bounds(objeto.transform.position,Vector3.zero);
            foreach (Transform child in objeto.transform) {
                childRender = child.GetComponent<Renderer>();
                if (childRender) {
                    bounds.Encapsulate(childRender.bounds);
                }else{
                    bounds.Encapsulate(getBounds(child.gameObject));
                }
            }
        }
        return bounds;
    }

    // Start is called before the first frame update
    void Start()
    {
        root = GameObject.Find("root");
        Bounds b = getBounds(root);
        // Bounds b = root.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh.bounds;

        // Vector3 offset = b.center;
        originalSize = b.size;
        curRoomWidth = originalSize.x;
        curRoomHeight = originalSize.y;
        curRoomDepth = originalSize.z;

        xText = GlobalFunctions.GetChildWithName(roomsizeGO, "xValue").GetComponent<TMP_InputField>();
        yText = GlobalFunctions.GetChildWithName(roomsizeGO, "yValue").GetComponent<TMP_InputField>();
        zText = GlobalFunctions.GetChildWithName(roomsizeGO, "zValue").GetComponent<TMP_InputField>();

        xSlider = GlobalFunctions.GetChildWithName(roomsizeGO, "xSlider").GetComponent<Slider>();
        ySlider = GlobalFunctions.GetChildWithName(roomsizeGO, "ySlider").GetComponent<Slider>();
        zSlider = GlobalFunctions.GetChildWithName(roomsizeGO, "zSlider").GetComponent<Slider>();

        SetPlayerTransform();
        audioSourceManager.SetOriginalRoomDimensions (originalSize.x, originalSize.y, originalSize.z);
    }

    public void SetRoomSizeThroughSliderVals(float x, float y, float z)
    {
        Debug.Log(x + " " + y + " " + z);
        xSlider.SetValueWithoutNotify(x);
        SetRoomX(x);
        ySlider.SetValueWithoutNotify(y);
        SetRoomY(y);
        zSlider.SetValueWithoutNotify(z);
        SetRoomZ(z);
    }

    public void SetRoomSizeSliders (Vector3 scalingToSet)
    {
        xSlider.SetValueWithoutNotify(scalingToSet.x);
        xText.text = scalingToSet.x.ToString("0.0");
        curRoomWidth = scalingToSet.x * originalSize.x;
        SetRoomX (scalingToSet.x);

        ySlider.SetValueWithoutNotify(scalingToSet.y);
        yText.text = scalingToSet.y.ToString("0.0");
        curRoomHeight = scalingToSet.y * originalSize.y;
        SetRoomY (scalingToSet.y);

        zSlider.SetValueWithoutNotify(scalingToSet.z);
        zText.text = scalingToSet.z.ToString("0.0");
        curRoomDepth = scalingToSet.z * originalSize.z;
        SetRoomZ (scalingToSet.z);

    }

    public void SetRoomXFromString (string x)
    {
        float res;
        if (float.TryParse(x, out res))
        {
            res = Mathf.Max(1.0f, res);
            xSlider.SetValueWithoutNotify(res);
            SetRoomX(res);
        }
        else
        {
            xText.text = (curRoomWidth / originalSize.x).ToString("0.0");
        }

    }

    public void SetRoomYFromString(string y)
    {
        float res;
        if (float.TryParse(y, out res))
        {
            res = Mathf.Max(1.0f, res);
            ySlider.SetValueWithoutNotify(res);
            SetRoomY(res);
            prevYval = res;
        }
        else
        {
            yText.text = (curRoomHeight / originalSize.y).ToString("0.0");
        }
    }

    public void SetRoomZFromString(string z)
    {
        float res;
        if (float.TryParse(z, out res))
        {
            res = Mathf.Max(1.0f, res);
            zSlider.SetValueWithoutNotify(res);
            SetRoomZ(res);
        }
        else
        {
            zText.text = (curRoomDepth / originalSize.z).ToString("0.0");
        }
    }

    public void SetRoomX (float x)
    {
        root.transform.localScale = new Vector3(x, root.transform.localScale.y, root.transform.localScale.z);
        xText.text = x.ToString("0.0");

        curRoomWidth = x * originalSize.x;
        
        audioSourceManager.SetSourcesFromRoomSizeX (curRoomWidth);
        playerManager.SetPlayerX(playerX, curRoomWidth);
        // speakerWallManager.UpdateSpeakerGrid();
    }

    public void SetRoomY(float y)
    {
        root.transform.localScale = new Vector3(root.transform.localScale.x, y, root.transform.localScale.z);
        yText.text = y.ToString("0.0");

        curRoomHeight = y * originalSize.y;

        audioSourceManager.SetSourcesFromRoomSizeY (curRoomHeight);
        // speakerWallManager.UpdateSpeakerGrid();
    }

    public void SetRoomZ(float z)
    {
        root.transform.localScale = new Vector3(root.transform.localScale.x, root.transform.localScale.y, z);
        zText.text = z.ToString("0.0");

        curRoomDepth = z * originalSize.z;

        audioSourceManager.SetSourcesFromRoomSizeZ (curRoomDepth);
        playerManager.SetPlayerZ(playerZ, curRoomDepth);
        // speakerWallManager.UpdateSpeakerGrid();
    }
    public void SetPlayerTransform()
    {
        playerX = (playerGO.transform.position.x - root.transform.localPosition.x) / curRoomWidth;
        playerZ = (playerGO.transform.position.z - root.transform.localPosition.z) / curRoomDepth;
    }

    public Vector3 GetNormalisedPlayerPos()
    {
        return new Vector3 (
            (playerGO.transform.position.x - root.transform.localPosition.x) / (curRoomWidth - playerManager.playerColliderDiameter) + 0.5f,
            (playerGO.transform.position.y - root.transform.localPosition.y) / curRoomHeight,
            (playerGO.transform.position.z - root.transform.localPosition.z) / (curRoomDepth - playerManager.playerColliderDiameter) + 0.5f
        );

    }

    // public void SetSourceTransform (Vector3 pos, int sourceIdx)
    // {
    //     sourcePositions[sourceIdx] = (new Vector3 (
    //         (pos.x - root.transform.localPosition.x) / roomWidth,
    //         (pos.y - root.transform.localPosition.y) / roomHeight,
    //         (pos.z - root.transform.localPosition.z) / roomDepth)
    //     );
    // }


    // public void SetSourceTransforms()
    // {
    //     for (int i = 0; i < sourcePositions.Count; ++i)
    //         SetSourceTransform(sourcePositions[i], i);
    // }

    //public void SetPlayerY(float y)
    //{
    //    playerY = y / roomHeight + 0.5f;
    //}

    //public void SetPlayerZ(float z)
    //{
    //    playerZ = z / roomDepth + 0.5f;
    //}
}
