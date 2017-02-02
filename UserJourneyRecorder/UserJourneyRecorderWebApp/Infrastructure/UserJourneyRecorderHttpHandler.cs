using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using System.Web.SessionState;

namespace UserJourneyRecorderWebApp.Infrastructure
{
    public class UserJourneyRecorderHttpHandler : HttpTaskAsyncHandler, IRequiresSessionState
    {
        public override Task ProcessRequestAsync(HttpContext context)
        {
            Task task = null;

            try
            {
                task = new Task(() =>
                {
                    if (context.Request.HttpMethod == "GET")
                    {
                        ProcessGetRequest(context);
                    }
                    else if (context.Request.HttpMethod == "POST")
                    {
                        ProcessPostRequest(context);
                    }

                    context.ApplicationInstance.CompleteRequest();
                });

                task.RunSynchronously();
            }
            catch (HttpException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.ApplicationInstance.CompleteRequest();
            }
            catch (ArgumentException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.ApplicationInstance.CompleteRequest();
            }

            return task;
        }

        private void ProcessGetRequest(HttpContext context)
        {
            var streamId = context.Request.Params["id"];
            var streamsDirectory = Utils.GetStreamsDirectory(context.Request.PhysicalApplicationPath);
            Utils.EnsureStreamDirectoryExists(streamsDirectory);
            var streamPath = Utils.GetStreamPath(streamsDirectory, streamId);
            var deleteStream = true;
            var autoDeleteStream = context.Request.Params["autodelete"];

            if (!string.IsNullOrEmpty(autoDeleteStream) && autoDeleteStream.ToLower() == false.ToString().ToLower())
            {
                deleteStream = false;
            }

            try
            {
                string payload;

                using (var inputStream = new FileStream(streamPath, FileMode.Open, FileAccess.Read))
                {
                    var inputReader = new StreamReader(inputStream, new UTF8Encoding());
                    payload = inputReader.ReadToEnd();
                    inputReader.Close();
                    inputStream.Close();
                }

                if (deleteStream)
                {
                    File.Delete(streamPath);
                }

                context.Response.AddHeader("Access-Control-Allow-Origin", "*");
                context.Response.AddHeader("Cache-Control", "no-cache");
                context.Response.AddHeader("Expires", "Thu, 01 Jan 1970 00:00:00 GMT");
                context.Response.AddHeader("Pragma", "no-cache");
                context.Response.Output.Write(payload);
                context.Response.StatusCode = (int)HttpStatusCode.OK;
            }
            catch (FileNotFoundException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NoContent;
            }
        }

        private void ProcessPostRequest(HttpContext context)
        {
            var streamId = context.Request.Params["id"];
            var streamsDirectory = Utils.GetStreamsDirectory(context.Request.PhysicalApplicationPath);
            Utils.EnsureStreamDirectoryExists(streamsDirectory);
            var streamPath = Utils.GetStreamPath(streamsDirectory, streamId);
            string payload;

            using (var inputReader = new StreamReader(context.Request.InputStream, Encoding.UTF8))
            {
                payload = inputReader.ReadToEnd();
            }

            using (var outputStream = new FileStream(streamPath, FileMode.Append, FileAccess.Write))
            {
                var outputWriter = new StreamWriter(outputStream, new UTF8Encoding());
                outputWriter.Write(payload);
                outputWriter.Flush();
                outputWriter.Close();
                outputStream.Close();
            }
        }
    }
}
