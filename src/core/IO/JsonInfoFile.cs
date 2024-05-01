using System.Text.Json;

namespace Jumpvalley.IO
{
    /// <summary>
    /// Class intended to help with using the data from info files written in JavaScript Object Notation (JSON).
    /// <br/><br/>
    /// Info files are files that contain metadata about an object (e.g. one that's on a filesystem).
    /// </summary>
    public partial class JsonInfoFile
    {
        /// <summary>
        /// The full file name of a JSON info file stored somewhere on a filesystem, including the file extension.
        /// </summary>
        public static readonly string FILE_NAME = "info.json";

        /// <summary>
        /// The raw JSON text from the JSON info file
        /// </summary>
        public string RawJson;

        /// <summary>
        /// Reads a JSON info file from its raw text and stores such data in the <see cref="RawData"/> field.
        /// </summary>
        /// <param name="json"></param>
        public JsonInfoFile(string json)
        {
            RawJson = json;
        }
    }
}
