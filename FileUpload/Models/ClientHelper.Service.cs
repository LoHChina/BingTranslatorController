using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

public static class ServiceClientHelper
{

    public static void createHandBack(string packagePath, string parentId, string handbackName ,string ServiceEndpoint, string requestUri)
    {
        using (var fileStream = File.OpenRead(packagePath))
        {
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var requestContent = new MultipartFormDataContent();
            requestContent.Add(fileContent, "handback_package", Path.GetFileName(packagePath));
            requestContent.Add(new StringContent(handbackName), "name");
            requestContent.Add(new StringContent(parentId), "parent_id");

            var result = PostAsync<Object>(ServiceEndpoint, requestUri, requestContent).Result;

        }
    }


    public static async Task<Object> PostAsync<T>(string baseuri, string uri, HttpContent content)
    {
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(baseuri);
            SetupHeader(client);
            string url = baseuri + uri;
            var response = await client.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                var resultStr = await response.Content.ReadAsStringAsync();
                return resultStr;
            }

            throw new Exception("PostAsync Exception:  \n" + response.ToString());
        }
    }

 

    #region Private Methods

    private static void SetupHeader(HttpClient client)
    {
        if (client == null)
        {
            return;
        }

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "alps@microsoft.com");
        client.DefaultRequestHeaders.Add("TokenType", "jwt");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
    #endregion
}

