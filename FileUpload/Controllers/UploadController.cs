using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

public class UploadController : ApiController
{
    public async Task<HttpResponseMessage> PostFile()
    {

        string root = HttpContext.Current.Server.MapPath("~/App_Data");
        var provider = new MultipartFormDataStreamProvider(root);
        HttpResponse response=HttpContext.Current.Response;
        response.AppendHeader("Access-Control-Allow-Origin", "*");
        response.AppendHeader("Access-Control-Allow-Method", "*");
        HttpRequest request= HttpContext.Current.Request;
        //string filename=request["filename"];
        try
        {
            StringBuilder sb = new StringBuilder(); // Holds the response body

            // Read the form data and return an async task.
            await Request.Content.ReadAsMultipartAsync(provider);

            string [] filename = provider.FormData.GetValues("FileName");
            string [] projectId = provider.FormData.GetValues("projectId");
             // This illustrates how to get the file names for uploaded files.
            foreach (var file in provider.FileData)
            {
                FileInfo fileInfo = new FileInfo(file.LocalFileName);
                XlifParser xliffParser = new XlifParser();
                string info = "";
                if (projectId != null)
                {
                    info = xliffParser.run(fileInfo.FullName, filename[0], projectId[0]);
                }
                else
                {
                    info = xliffParser.run(fileInfo.FullName, filename[0]);
                }
                
                sb.Append(info);

            }
            return new HttpResponseMessage()
            {
                Content = new StringContent(sb.ToString())
            };
        }
        catch (System.Exception e)
        {
            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
        }
    }


}