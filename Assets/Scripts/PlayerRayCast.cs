using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerRayCast : MonoBehaviour
{
    // Start is called before the first frame update

    public TMP_Text selectedWallText;
    public TMP_Text selectedMaterialText;
    public TMP_Dropdown dropdown;

    private GeometryManager geometryManager;

    private GameManagerSteamAudio gameManager;

    private bool shiftDown, leftShiftDown, rightShiftDown = false;

    void Start()
    {
        geometryManager = GameObject.Find("GeometryManager").GetComponent<GeometryManager>();
        gameManager = GameObject.Find("MainCanvas").GetComponent<GameManagerSteamAudio>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("left shift"))
        {
            leftShiftDown = true;
        }
        if (Input.GetKeyUp("left shift"))
        {
            leftShiftDown = false;
        }
        if (Input.GetKeyDown("right shift"))
        {
            rightShiftDown = true;
        }
        if (Input.GetKeyUp("right shift"))
        {
            rightShiftDown = false;
        }

        shiftDown = (leftShiftDown || rightShiftDown);
        if (Input.GetMouseButtonDown(0) && !gameManager.IsCanvasActive())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (int.TryParse(hit.transform.name.Substring(0, 1), out int value) )
                {
                    Debug.Log("Hit child " + hit.transform.name);
                    for (int o = 1; o < dropdown.options.Count; ++o)
                    {
                        if (dropdown.options[o].text == hit.transform.name.Substring (hit.transform.name.LastIndexOf('_') + 1))
                        {
                            Debug.Log(dropdown.options[o].text.Substring(dropdown.options[o].text.LastIndexOf('_') + 1));
                            dropdown.SetValueWithoutNotify (o);
                        }
                    }

                    // If wall is already selected remove it from the list
                    if (geometryManager.GetSelectedWalls().Contains (hit.transform.parent))
                    {
                        if (shiftDown || geometryManager.GetSelectedWalls().Count == 1)
                            geometryManager.GetSelectedWalls().Remove (hit.transform.parent);
                        else
                        {
                            geometryManager.ClearSelectedWalls();
                            geometryManager.AddSelectedWall (hit.transform.parent);
                        }
                    }
                    else
                    {
                        // If shift is not down, clear wall list first
                        if (!shiftDown)
                            geometryManager.ClearSelectedWalls();

                        // Add the selected wall to the list
                        geometryManager.AddSelectedWall (hit.transform.parent);
                    }

                    // Set the selected children active 
                    // foreach (Transform child in hit.transform.parent.parent)
                    // {
                    //     // Is child selected
                    //     bool childIsSelected = geometryManager.GetSelectedWalls().Contains (child);
                    //     Debug.Log ("Is child selected? " + childIsSelected);
                    //     child.GetChild(hit.transform.parent.childCount - 1).gameObject.SetActive(childIsSelected);
                    // }

                    if (geometryManager.GetSelectedWalls().Count == 0)
                    {
                        selectedWallText.text = "Selected wall: None";
                        selectedMaterialText.text = "Selected material: None";
                    }
                    else if (geometryManager.GetSelectedWalls().Count == 1)
                    {
                        selectedWallText.text = "Selected wall: " + geometryManager.GetSelectedWalls()[0].name;
                        selectedMaterialText.text = "Selected material: " + geometryManager.GetActiveMaterialOf(0);
                    } else {
                        selectedWallText.text = "Selected wall: Multiple (" + geometryManager.GetSelectedWalls().Count + ")";
                        selectedMaterialText.text = "Selected material: Multiple (" + geometryManager.GetSelectedWalls().Count + ")";
                    }
                }
            }
        }
    }
}
