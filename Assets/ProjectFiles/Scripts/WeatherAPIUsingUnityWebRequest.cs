using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System;
using System.Collections;
using System.Net;

public class WeatherAPIUsingUnityWebRequest : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cityText;
    [SerializeField] private TextMeshProUGUI temperatureText;
    [SerializeField] private TextMeshProUGUI weatherDataText;
    [SerializeField] private TMP_InputField cityNameInputField;
    [SerializeField] private GameObject[] weatherIcons;
    [Tooltip("Text which shows if the city entered is wrong or misspelled.")]
    [SerializeField] private TextMeshProUGUI errorMessage;

    private string apiKey = "047395d64389e388367bd602b9a18096";
    private string baseUrl = "https://api.openweathermap.org/data/2.5/weather?q=";

    private void Start()
    {
        cityNameInputField.onEndEdit.AddListener(OnCityNameEntered);
    }

    private void OnCityNameEntered(string cityName)
    {
        StartCoroutine(GetWeather(cityName));
    }

    private IEnumerator GetWeather(string cityName)
    {
        string url = $"{baseUrl}{cityName}&appid={apiKey}";

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            ShowErrorMessage("Error occurred while fetching weather data.");
            //Debug.LogError("HTTP Request Exception: " + request.error);
        }
        else
        {
            if (request.responseCode == 404)
            {
                ShowErrorMessage("City not found. Please enter a valid city name.");
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("JSON Response: " + jsonResponse);
                TurnOnOtherUI();

                try
                {
                    JsonData weatherData = JsonUtility.FromJson<JsonData>(jsonResponse);

                    errorMessage.gameObject.SetActive(false);

                    float temperatureInKelvin = weatherData.main.temp;
                    float temperatureInCelsius = temperatureInKelvin - 273.15f;
                    int temp = (int)temperatureInCelsius;

                    temperatureText.text = temp.ToString() + "°C";

                    cityText.text = cityName;

                    if (weatherData.weather != null && weatherData.weather.Length > 0)
                    {
                        foreach (var weather in weatherIcons)
                        {
                            weather.gameObject.SetActive(false);
                        }

                        string weatherCondition = weatherData.weather[0].main;

                        foreach (GameObject icon in weatherIcons)
                        {
                            if (icon.CompareTag(weatherCondition))
                            {
                                icon.SetActive(true);
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowErrorMessage("Error parsing weather data.");
                    Debug.LogError("Exception: " + ex.Message);
                }
            }
        }
    }


    private void ShowErrorMessage(string message)
    {
        errorMessage.gameObject.SetActive(true);
        errorMessage.text = message;
        TurnOffOtherUI();
    }

    private void TurnOffOtherUI()
    {
        cityText.gameObject.SetActive(false);
        temperatureText.gameObject.SetActive(false);
        weatherDataText.gameObject.SetActive(false);
        foreach (var weather in weatherIcons)
        {
            weather.gameObject.SetActive(false);
        }
    }

    private void TurnOnOtherUI()
    {
        cityText.gameObject.SetActive(true);
        temperatureText.gameObject.SetActive(true);
        weatherDataText.gameObject.SetActive(true);
    }
}
