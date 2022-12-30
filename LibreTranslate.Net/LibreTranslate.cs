using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LibreTranslate.Net.Models;
using Newtonsoft.Json;

namespace LibreTranslate.Net;

public class LibreTranslate
{
    /// <summary>
    /// The http client
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// The default contructor. The default http client base uri points to https://libretranslate.com
    /// </summary>
    public LibreTranslate()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://libretranslate.com")
        };
    }

    /// <summary>
    /// Contructor which enable to specified the libre translate server api address
    /// </summary>
    /// <param name="url"></param>
    public LibreTranslate(string url)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(url)
        };
    }

    /// <summary>
    /// Detect the language of a single text.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<DetectResponse>> DetectLanguagesAsync(Detect detect,
        CancellationToken? cancellationToken = null)
    {
        try
        {
            var formData = new Dictionary<string, string>
            {
                { "q", detect.Text }
            };

            if (detect.ApiKey is not null)
                formData.Add("api_key", detect.ApiKey);

            var formUrlEncodedContent = new FormUrlEncodedContent(formData);

            var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, "/detect")
            {
                Content = formUrlEncodedContent
            }, cancellationToken ?? new CancellationToken());

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Response status: " + string.Join(", ", response.StatusCode, content));

            var languages =
                JsonConvert.DeserializeObject<IEnumerable<DetectResponse>>(await response.Content.ReadAsStringAsync());
            
            return languages ?? Enumerable.Empty<DetectResponse>();
        }
        catch
        {
            return Enumerable.Empty<DetectResponse>();
        }
    }

    /// <summary>
    /// Gets the server supported languages.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<SupportedLanguages>> GetSupportedLanguagesAsync()
    {
        try
        {
            var responseContent = await _httpClient.GetStringAsync("/languages");
            var languages = JsonConvert.DeserializeObject<IEnumerable<SupportedLanguages>>(responseContent);

            return languages ?? Enumerable.Empty<SupportedLanguages>();
        }
        catch
        {
            return Enumerable.Empty<SupportedLanguages>();
        }
    }

    /// <summary>
    /// Translates the text from one language to another.
    /// </summary>
    public async Task<string?> TranslateAsync(Translate translate,
        CancellationToken? cancellationToken = null)
    {
        var formData = new Dictionary<string, string>
        {
            { "q", translate.Text },
            { "source", translate.Source.ToString() },
            { "target", translate.Target.ToString() }
        };

        var formUrlEncodedContent = new FormUrlEncodedContent(formData);

        if (translate.ApiKey is not null)
            formData.Add("api_key", translate.ApiKey);

        var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, "/translate")
        {
            Content = formUrlEncodedContent
        }, cancellationToken ?? new CancellationToken());

        if (!response.IsSuccessStatusCode) return null;

        var translationResponse =
            JsonConvert.DeserializeObject<TranslationResponse>(await response.Content.ReadAsStringAsync());

        return translationResponse?.TranslatedText;
    }
}