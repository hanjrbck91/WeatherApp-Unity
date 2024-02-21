using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeatherAPICall : MonoBehaviour
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
    private HttpClient httpClient = new HttpClient();

    private void Start()
    {
        // Attach an event listener to the InputField to trigger the API call
        cityNameInputField.onEndEdit.AddListener(OnCityNameEntered);
    }
  

    private async void OnCityNameEntered(string cityName)
    {
        // Construct the URL with the city name and API key
        string url = $"{baseUrl}{cityName}&appid={apiKey}";

        try
        {
            HttpResponseMessage response = await httpClient.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                ShowErrorMessage("City not found. Please enter a valid city name.");
                return;
            }

            response.EnsureSuccessStatusCode(); // Throw if not a success code

            string jsonResponse = await response.Content.ReadAsStringAsync();
            Debug.Log("JSON Response: " + jsonResponse);
            TurnOnOtherUI();

            // Deserialize JSON response into a C# object
            JsonData weatherData = JsonUtility.FromJson<JsonData>(jsonResponse);

            // Update UI with weather data
            if (!System.Object.ReferenceEquals(temperatureText,null))
            {
                // setting of the errormessage from ui since the city is correct
                errorMessage.gameObject.SetActive(false);
                // Convert temperature from Kelvin to Celsius
                float temperatureInKelvin = weatherData.main.temp;
                float temperatureInCelsius = temperatureInKelvin - 273.15f;
                int temp = (int)temperatureInCelsius;

                temperatureText.text = temp.ToString() + "°C"; // Display temperature with 2 decimal places
            }
            if (cityNameInputField is not null)
            {
                cityText.text = cityNameInputField.text.ToString();
            }
            if (weatherDataText != null && weatherData != null && weatherData.weather != null && weatherData.weather.Length > 0)
            {
                foreach (var weather in weatherIcons)
                {
                    weather.gameObject.SetActive(false);
                }
                // Get the main weather condition as a string
                string weatherCondition = weatherData.weather[0].main;

                // Display the main weather condition
               // weatherDataText.text = weatherCondition;


                // Find the game object with the corresponding tag in the weather icon array
                GameObject matchingIcon = null;

                foreach (GameObject icon in weatherIcons)
                {
                    if (icon.CompareTag(weatherCondition))
                    {
                        matchingIcon = icon;
                        break;
                    }
                }

                // Activate the matching game object if found
                if (matchingIcon != null)
                {
                    matchingIcon.SetActive(true);
                }
                else
                {
                    Debug.LogWarning("No game object found for weather condition: " + weatherCondition);
                }
            }
            //else
            //{
            //    Debug.LogError("Weather data or weatherDataText is null or invalid.");
            //}

            else
            {
                Debug.LogError("Temperature Text component is not assigned.");
            }
        }
        catch (HttpRequestException e)
        {
            ShowErrorMessage("Error occurred while fetching weather data.");
            Debug.LogError("HTTP Request Exception: " + e.Message);
        }
        catch (Exception ex)
        {
            Debug.LogError("Exception: " + ex.Message);
        }
    }

    private void ShowErrorMessage(string message)
    {
        errorMessage.gameObject.SetActive(true);
        errorMessage.text = message;
        TurnOffOtherUI();
    }

    private void OnDestroy()
    {
        httpClient.Dispose(); // Dispose HttpClient to free resources
    }

    void TurnOffOtherUI()
    {
        cityText.gameObject.SetActive(false);
        temperatureText.gameObject.SetActive(false);
        weatherDataText.gameObject.SetActive(false);
        foreach (var weather in weatherIcons)
        {
            weather.gameObject.SetActive(false);
        }
    }
    void TurnOnOtherUI()
    {
        cityText.gameObject.SetActive(true);
        temperatureText.gameObject.SetActive(true);
        weatherDataText.gameObject.SetActive(true);
    }
}
