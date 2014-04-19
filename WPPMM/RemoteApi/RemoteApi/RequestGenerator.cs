using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace WPPMM.RemoteApi
{
    /// <summary>
    /// This class provide function to generate JSON string to set as a body of API request.
    /// You just have to set required argument for the APIs.
    /// </summary>
    internal class RequestGenerator
    {
        private static int request_id = 0;

        private static int GetID()
        {
            var id = Interlocked.Increment(ref request_id);
            if (request_id > 1000000000)
            {
                request_id = 0;
            }
            return id;
        }

        private static string CreateJson(string name, string version, params object[] prms)
        {
            var param = new JArray();
            foreach (var p in prms)
            {
                param.Add(p);
            }

            var json = new JObject(
                new JProperty("method", name),
                new JProperty("version", version),
                new JProperty("id", GetID()),
                new JProperty("params", param));

            return json.ToString().Replace(" ", "").Replace("\n", "").Replace("\r", "");
        }

        /// <summary>
        /// Automatically insert "version":"1.0"
        /// </summary>
        /// <param name="name"></param>
        /// <param name="prms"></param>
        /// <returns></returns>
        internal static string Jsonize(string name, params object[] prms)
        {
            return Jsonize(name, Version.V1_0, prms);
        }

        internal static string Jsonize(string name, Version version, params object[] prms)
        {
            return CreateJson(name, ToString(version), prms);
        }

        private static string ToString(Version version)
        {
            switch (version)
            {
                case Version.V1_0:
                    return "1.0";
                case Version.V1_1:
                    return "1.1";
                default:
                    throw new System.InvalidOperationException("Version " + version + " is not supported.");
            }
        }
    }

    internal enum Version
    {
        V1_0,
        V1_1
    }
}
