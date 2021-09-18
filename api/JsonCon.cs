using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
namespace api
{
    /// <summary>
    /// returns deserialised JSON as a collection
    /// </summary>
    ///<returns>
    /// List of type T
    ///</returns>
    public class JsonCon
    {
        public List<T> deSerializeTextfileAsList<T>(string Path)
        // returns a collection of transactions
        {
            List<T> jsonList = JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(Path));
            return jsonList;
        }

        /// <summary>
        /// Returns a collection of transactions of type T.
        /// </summary>
        public List<T> deSerializeAsList<T>(string json)
        {
            List<T> jsonList = JsonConvert.DeserializeObject<List<T>>(json);
            return jsonList;
        }
    }
}