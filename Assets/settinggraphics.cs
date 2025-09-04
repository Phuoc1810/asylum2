using UnityEngine;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Rendering;
public class settinggraphics : MonoBehaviour
{
    public RenderPipelineAsset[] qualytilever;
    public TMP_Dropdown resdropdown;
    public TMP_Dropdown qualytidrop;
    public Toggle Fullscreen;

    Resolution[] resolutions;
    bool isfullscreen;
    int selectedresolutions;
    List<Resolution> selectedresolutionList = new List<Resolution>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        qualytidrop.value = QualitySettings.GetQualityLevel();
        isfullscreen = true;
        resolutions = Screen.resolutions;
        List<string> resolutionsStringList = new List<string>();
        string newres;
        foreach (Resolution res in resolutions)
        {
            newres=res.width.ToString()+"x"+res.height.ToString();
            if (!resolutionsStringList.Contains(newres))
            {
                resolutionsStringList.Add(newres);
                selectedresolutionList.Add(res);
            }
        }
        resdropdown.AddOptions(resolutionsStringList);
    }
    public void changeresolutions()
    {
        selectedresolutions =resdropdown.value;
        Screen.SetResolution(selectedresolutionList[selectedresolutions].width, selectedresolutionList[selectedresolutions].height,isfullscreen);
    }
   public void changefullscreen()
    {
        isfullscreen = Fullscreen.isOn;
        Screen.SetResolution(selectedresolutionList[selectedresolutions].width, selectedresolutionList[selectedresolutions].height, isfullscreen);
    }
    public void changequality(int value)
    {
        QualitySettings.SetQualityLevel(value);
        QualitySettings.renderPipeline = qualytilever[value];
    }
    void Update()
    {
        
    }
}
