using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARKit;
using UnityEngine.XR.ARSubsystems;


[RequireComponent(typeof(ARFaceManager))]
public class FaceSwitchController : MonoBehaviour
{
    [SerializeField]
    private Button swapFacesToggle;

    private ARFaceManager arFaceManager;

    private ARKitFaceSubsystem faceSubsystem;

    private int swapCounter = 0;

    [SerializeField]
    public FaceMaterial[] materials;
    
    [SerializeField]
    private Button switchToggle;

    private bool toggle;

    private bool flag = false;

    private float a, b;

    void Awake() 
    {
        arFaceManager = GetComponent<ARFaceManager>();
        if (arFaceManager != null) {
            faceSubsystem = (ARKitFaceSubsystem)arFaceManager.subsystem;
        }
        
        // faceTrackingToggle.onClick.AddListener(ToggleTrackingFaces);
        swapFacesToggle.onClick.AddListener(SwapFaces);
        switchToggle.onClick.AddListener(ToggleOnOff);

        arFaceManager.facePrefab.GetComponent<MeshRenderer>().material = materials[0].Material;
    }

    void SwapFaces() 
    { 
        swapCounter = swapCounter == materials.Length - 1 ? 0 : swapCounter + 1;
        
        foreach(ARFace face in arFaceManager.trackables)
        {
            face.GetComponent<MeshRenderer>().material = materials[swapCounter].Material;
        }

        // swapFacesToggle.GetComponentInChildren<Text>().text = $"Face Material ({materials[swapCounter].Name})";
    }

    void ToggleOnOff()
    {   
        Debug.Log("pressed");
        toggle = !toggle;
        if(toggle){
            switchToggle.GetComponentInChildren<Text>().text = "Auto: ON";
        }else{
            switchToggle.GetComponentInChildren<Text>().text = "Auto: OFF";
        }
        
    }

    void Update()
    {
        if(toggle){
            foreach(ARFace face in arFaceManager.trackables)
            {
                using (var blendShapes = faceSubsystem.GetBlendShapeCoefficients(face.trackableId, Allocator.Temp)) {
                    
                    foreach(var shape in blendShapes)
                    {
                        if(shape.blendShapeLocation == ARKitBlendShapeLocation.EyeBlinkLeft)
                        {
                            a = shape.coefficient;
                        }
                        if(shape.blendShapeLocation == ARKitBlendShapeLocation.EyeBlinkRight)
                        {
                            b = shape.coefficient;
                        }
                        
                    }

                    if (a > 0.8 && b > 0.8 && !flag){
                        flag = true;
                        SwapFaces();
                    }else{
                        flag = false;
                    }

                } 
            }
        }

    }
}

[System.Serializable]
public class FaceMaterial
{
    public Material Material;

    public string Name;
}