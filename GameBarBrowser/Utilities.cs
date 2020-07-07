using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace GameBarBrowser
{
    public class Utilities
    {
        private static readonly StorageFolder RoamingFolder = ApplicationData.Current.RoamingFolder;

        public static async Task<T> DeserialiseJson<T>(string fileName)
        {
            var file = await RoamingFolder.GetFileAsync(fileName);
            var content = await FileIO.ReadTextAsync(file);
            return JsonConvert.DeserializeObject<T>(content);
        }

        public static async Task<bool> SerialiseJson(string fileName, object obj)
        {
            try
            {
                string content = JsonConvert.SerializeObject(obj, Formatting.Indented);
                StorageFile file = await RoamingFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, content);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<bool> IsFilePresent(string fileName)
        {
            var item = await ApplicationData.Current.RoamingFolder.TryGetItemAsync(fileName);
            return item != null;
        }
    }
}
