using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace GameBarBrowser2
{
    public static class FileUtilities
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
            System.Diagnostics.Debug.WriteLine(RoamingFolder.Path);
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
            IStorageItem item = await ApplicationData.Current.RoamingFolder.TryGetItemAsync(fileName);
            return item != null;
        }
    }
}
