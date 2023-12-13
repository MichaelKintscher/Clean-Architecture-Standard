using CleanArchitectureStandard.Adapters.Json;
using CleanArchitectureStandard.Application;
using CleanArchitectureStandard.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CleanArchitectureWindows.Persistence
{
    /// <summary>
    /// Implements base storage provider functions for Windows.
    /// </summary>
    public class WindowsStorageProviderBase : Singleton<WindowsStorageProviderBase>, IStorageProvider<WindowsStorageProviderBase>
    {
        /// <summary>
        /// Saves a list of values of the given type.
        /// </summary>
        /// <typeparam name="T">The type of values to save.</typeparam>
        /// <param name="values">The values to save.</param>
        /// <param name="fileName">The name of the file to save the values to.</param>
        /// <returns></returns>
        public async Task SaveAsync<T>(List<T> values, string fileName)
        {
            // Serialize the data to JSON.
            string valuesString = EntityJsonAdapter<T>.SerializeList(values);

            // Create the file, replacing the old if it already exists, and write the data to the file.
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, valuesString);
        }

        /// <summary>
        /// Loads a list of values of the given type.
        /// </summary>
        /// <typeparam name="T">The type of values to load.</typeparam>
        /// <param name="fileName">The name of the file to load the values from.</param>
        /// <returns></returns>
        public async Task<List<T>> LoadAsync<T>(string fileName)
        {
            // Initialize the list.
            List<T> values = new List<T>();

            // Try to read the list from the file.
            IStorageItem storageItem = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
            if (storageItem is StorageFile file)
            {
                // Read the data from the file.
                string fileContent = await FileIO.ReadTextAsync(file);
                values = EntityJsonAdapter<T>.DeserializeList(fileContent);
            }

            return values;
        }

        /// <summary>
        /// Adds the given value to the saved collection of that value.
        /// </summary>
        /// <typeparam name="T">The type of the value to add.</typeparam>
        /// <param name="value">The value to add.</param>
        /// <param name="fileName">The name of the file to add the value to.</param>
        /// <returns></returns>
        public async Task AddAsync<T>(T value, string fileName)
        {
            List<T> values = await this.LoadAsync<T>(fileName);
            values.Add(value);
            await this.SaveAsync(values, fileName);
        }

        /// <summary>
        /// Adds the given values to the saved collection of those values.
        /// </summary>
        /// <typeparam name="T">The type of the value to add.</typeparam>
        /// <param name="values">The values to add.</param>
        /// <param name="fileName">The name of the file to add the value to.</param>
        /// <returns></returns>
        public async Task AddAsync<T>(IEnumerable<T> values, string fileName)
        {
            List<T> allValues = await this.LoadAsync<T>(fileName);
            allValues.AddRange(values);
            await this.SaveAsync(allValues, fileName);
        }

        /// <summary>
        /// Removes the given value from the saved collection of those values.
        /// </summary>
        /// <typeparam name="T">The type of the value to remove.</typeparam>
        /// <param name="value">The value to remove.</param>
        /// <param name="fileName">The name of the file to remove the value from.</param>
        /// <returns>The removed value, or the default value if nothing was removed.</returns>
        public async Task<T> RemoveAsync<T>(T value, string fileName)
        {
            List<T> allValues = await this.LoadAsync<T>(fileName);
            bool removed = allValues.Remove(value);
            await this.SaveAsync(allValues, fileName);

            return removed ? value : default;
        }

        /// <summary>
        /// Removes the given values from the saved collection of those values.
        /// </summary>
        /// <typeparam name="T">The type of the value to remove.</typeparam>
        /// <param name="values">The values to remove.</param>
        /// <param name="fileName">The name of the file to remove the values from.</param>
        /// <returns>The removed values, or an empty collection if nothing was removed.</returns>
        public async Task<IEnumerable<T>> RemoveAsync<T>(IEnumerable<T> values, string fileName)
        {
            List<T> removedValues = new List<T>();
            List<T> allValues = await this.LoadAsync<T>(fileName);
            foreach (T value in values)
            {
                if(allValues.Remove(value))
                {
                    removedValues.Add(value);
                }
            }
            await this.SaveAsync(allValues, fileName);

            IEnumerable<T> results = removedValues;
            return results;
        }
    }
}
