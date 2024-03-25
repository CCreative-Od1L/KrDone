using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Core.Utils {
    public class JsonUtils {
        // * Newtonsoft VERSION
        // static public T? ParseJsonString<T>(string jsonString, JsonSerializerSettings? options = null) {
        //     options ??= new JsonSerializerSettings() {
        //         StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
        //         NullValueHandling = NullValueHandling.Ignore,
        //     };
        //     return JsonConvert.DeserializeObject<T>(jsonString, options);
        // }
        // static public string SerializeJsonObj<T>(T jsonObj, JsonSerializerSettings? options = null) {
        //     options ??= new JsonSerializerSettings() {
        //         StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
        //         NullValueHandling = NullValueHandling.Ignore,
        //     };
        //     return JsonConvert.SerializeObject(jsonObj, options);
        // }
        // * Standard
        static public T? ParseJsonString<T>(string jsonString, JsonSerializerOptions? options = null) {
            options ??= new JsonSerializerOptions() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            };
            return JsonSerializer.Deserialize<T>(jsonString, options);
        }
        static public string SerializeJsonObj<T>(T jsonObj, JsonSerializerOptions? options = null) {
            options ??= new JsonSerializerOptions() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            };
            return JsonSerializer.Serialize(jsonObj, options);
        }
    }
}