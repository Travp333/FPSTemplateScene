//Author: Brian Meginness
//Debugging: Brian Meginness
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SettingsMenu : MonoBehaviour
{
    //Menu components
    Slider volSlide;
    Dropdown resDrop;
    Toggle fullToggle;
    Dropdown qualDrop;

	//Resolution[] resolutions;

    // Start is called before the first frame update
    void Start()
    {
        //Get components
        volSlide = GameObject.Find("VolumeSlide").GetComponent<Slider>();
        volSlide.value = 0.5f;
        resDrop = GameObject.Find("Resolution").GetComponent<Dropdown>();
	    //resolutions = Screen.resolutions.Where(resolution => resolution.refreshRate == Screen.currentResolution.refreshRate).ToArray();
        fullToggle = GameObject.Find("Fullscreen").GetComponent<Toggle>();
        fullToggle.isOn = Screen.fullScreen;
        //qualDrop = GameObject.Find("Quality").GetComponent<Dropdown>();
        //Get available, current resolutions for resolutions dropdown
        GetResolutions();

        //GetQuality();
    }

    //On volume slider change
    public void changeVol()
    {
        //Slider OnChange() is called when initialized, sometimes before start() can finish
        if (volSlide)
        {
            AudioListener.volume = volSlide.value * 2;
        }

    }

    //Get available, current resolutions for resolutions dropdown
    private void GetResolutions()
    {
        //Find current resolution by index
	    //int currentResIndex = 0;

        //FOR all resolutions, add width x height to options
        List<string> options = new List<string>();
	    //for (int i = 0; i < resolutions.Length; i++)
	    //{
	    //    options.Add(resolutions[i].width + "x" + resolutions[i].height);
            //IF resolution i is current resolution, save index
	    //     if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
	    //     {
	    //        currentResIndex = i;
	    //     }
	    //  }

        //Add options to dropdown, set active to current resolution
	    //  resDrop.AddOptions(options);
	    //  resDrop.value = currentResIndex;
    }

    //Set screen resolution
    public void SetResolution(int resolutionIndex)
    {
        //Get resolution based on list index
	    // Resolution resolution = resolutions[resolutionIndex];
        //Set screen resolution
	    // Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    //Set whether window is fullscreen
    public void SetFullscreen(bool fullscreen)
    {
	    // Screen.SetResolution(Screen.width, Screen.height, fullscreen);
    }

  //  private void GetQuality()
  //  {
        //Get current quality index
   //     int currentQuality = QualitySettings.GetQualityLevel();

        //IF there are available quality settings
    //    if (QualitySettings.names.Length > 0) {
            //Remove placeholder
    //        qualDrop.ClearOptions();
            //Set dropdown options to all available quality settings
   //         qualDrop.AddOptions(new List<string>(QualitySettings.names));
   //     }
   // }

    //Set graphics engine quality preset
    public void setQuality(int quality)
    {
        QualitySettings.SetQualityLevel(quality);
    }
}
