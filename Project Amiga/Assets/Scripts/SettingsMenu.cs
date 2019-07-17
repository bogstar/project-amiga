using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Amiga.UI
{
	public class SettingsMenu : HUDPanel
	{
		public Text title;
		public Dropdown resolutionDropdown;
		public Toggle fullscreenToggle;

		public Slider volumeSlider;
		public Text volumeSliderText;

		public Button applyButton;

		public AudioMixerGroup mixer;

		Resolution[] resolutions;


		protected override void Start()
		{
			base.Start();
			fullscreenToggle.onValueChanged.AddListener(delegate { OnAnyValueChanged(); });
			resolutionDropdown.onValueChanged.AddListener(delegate { OnAnyValueChanged(); });
			volumeSlider.onValueChanged.AddListener(delegate { OnAnyValueChanged(); });
			volumeSlider.onValueChanged.AddListener(delegate { Input_OnVolumeChange(volumeSlider.value); });
			applyButton.interactable = false;
		}

		void OnEnable()
		{
			resolutions = Screen.resolutions;
			Resolution currentResolution = new Resolution
			{
				height = Screen.height,
				width = Screen.width
			};

			List<Dropdown.OptionData> optionData = new List<Dropdown.OptionData>();

			int currRes = -1;
			int i = 0;

			foreach (var res in resolutions)
			{
				if (res.height == currentResolution.height && res.width == currentResolution.width)
				{
					currRes = i;
				}

				optionData.Add(new Dropdown.OptionData(res.width + "x" + res.height));
				i++;
			}

			resolutionDropdown.AddOptions(optionData);
			resolutionDropdown.value = currRes;
			resolutionDropdown.RefreshShownValue();
			fullscreenToggle.isOn = Screen.fullScreen;

			int currentVolume = GameManager.Instance.GetVolumeLevel();
			volumeSlider.value = currentVolume / 100f;
			volumeSliderText.text = currentVolume.ToString();
		}

		void OnDisable()
		{
			resolutionDropdown.ClearOptions();
		}

		void OnAnyValueChanged()
		{
			applyButton.interactable = true;
		}

		public void Input_HideSettings()
		{
			GameManager.Instance.CurrentSceneSettings.HideTopPanel();
		}

		public void Input_OnVolumeChange(float value)
		{
			float currentVolume = volumeSlider.value;
			volumeSliderText.text = ((int)(currentVolume * 100)).ToString();
		}

		public void Input_ApplyChanges()
		{
			applyButton.interactable = false;

			Resolution newRes = resolutions[resolutionDropdown.value];

			GameManager.Instance.SetResolution(newRes, fullscreenToggle.isOn);

			float parameter = volumeSlider.value * .55f * 100f;

			parameter -= 55f;

			GameManager.Instance.SetVolumeLevel(parameter, (int)(volumeSlider.value * 100f));
		}
	}
}