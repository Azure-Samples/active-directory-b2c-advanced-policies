using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using UserJourneyRecorderWebApp.Properties;

namespace UserJourneyRecorderWebApp.Infrastructure
{
    internal static class Utils
    {
        public static void EnsureStreamDirectoryExists(string streamPath)
        {
            Directory.CreateDirectory(streamPath);
        }

        public static string GetStreamsDirectory(string basePath)
        {
            return basePath + "streams\\";
        }

        public static string GetStreamPath(string streamsDirectory, string streamId)
        {
            ValidateStreamId(streamId);
            return streamsDirectory + streamId;
        }

        public static void ValidateStreamId(string streamId)
        {
            if (!Regex.IsMatch(streamId, "", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100)))
            {
                var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = string.Format(Resources.InvalidStreamId, streamId)
                };

                throw new HttpResponseException(errorResponse);
            }
        }
    }
}
