using CleanArchitecture.Core.Adapters.Json;
using CleanArchitecture.Core.Application;
using CleanArchitecture.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CleanArchitecture.Windows.Persistence
{
    /// <summary>
    /// Implements base storage provider functions for Windows.
    /// </summary>
    public class WindowsStorageProviderBase<T> : Singleton<T>, IStorageProvider where T : new()
    {
        /// <summary>
        /// Saves a list of values of the given type.
        /// </summary>
        /// <typeparam name="T">The type of values to save.</typeparam>
        /// <param name="values">The values to save.</param>
        /// <param name="fileName">The name of the file to save the values to.</param>
        /// <param name="adapter">The adapter to use to serialize the values. Defaults to EntityJsonAdapter if none is provided.</param>
        /// <returns></returns>
        public async Task SaveAsync<T>(List<T> values, string fileName, IJsonAdapter<T>? adapter = null)
        {
            // Serialize the data to JSON.
            string valuesString = adapter == null ? EntityJsonAdapter<T>.SerializeList(values) : adapter.SerializeList(values);

            // Create the file, replacing the old if it already exists, and write the data to the file.
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, valuesString);
        }

        /// <summary>
        /// Loads a list of values of the given type.
        /// </summary>
        /// <typeparam name="T">The type of values to load.</typeparam>
        /// <param name="fileName">The name of the file to load the values from.</param>
        /// <param name="adapter">The adapter to use to serialize the values. Defaults to EntityJsonAdapter if none is provided.</param>
        /// <returns></returns>
        public async Task<List<T>> LoadAsync<T>(string fileName, IJsonAdapter<T>? adapter = null)
        {
            // Initialize the list.
            List<T> values = new List<T>();

            // Try to read the list from the file.
            IStorageItem storageItem = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
            if (storageItem is StorageFile file)
            {
                // Read the data from the file.
                string fileContent = await FileIO.ReadTextAsync(file);
                values = adapter == null ? EntityJsonAdapter<T>.DeserializeList(fileContent) : adapter.DeserializeList(fileContent);
            }

            return values;
        }

        /// <summary>
        /// Adds the given value to the saved collection of that value.
        /// </summary>
        /// <typeparam name="T">The type of the value to add.</typeparam>
        /// <param name="value">The value to add.</param>
        /// <param name="fileName">The name of the file to add the value to.</param>
        /// <param name="adapter">The adapter to use to serialize the values. Defaults to EntityJsonAdapter if none is provided.</param>
        /// <returns></returns>
        public async Task AddAsync<T>(T value, string fileName, IJsonAdapter<T>? adapter = null)
        {
            List<T> values = await this.LoadAsync(fileName, adapter);
            values.Add(value);
            await this.SaveAsync(values, fileName, adapter);
        }

        /// <summary>
        /// Adds the given values to the saved collection of those values.
        /// </summary>
        /// <typeparam name="T">The type of the value to add.</typeparam>
        /// <param name="values">The values to add.</param>
        /// <param name="fileName">The name of the file to add the value to.</param>
        /// <param name="adapter">The adapter to use to serialize the values. Defaults to EntityJsonAdapter if none is provided.</param>
        /// <returns></returns>
        public async Task AddAsync<T>(IEnumerable<T> values, string fileName, IJsonAdapter<T>? adapter = null)
        {
            List<T> allValues = await this.LoadAsync(fileName, adapter);
            allValues.AddRange(values);
            await this.SaveAsync(allValues, fileName, adapter);
        }

        /// <summary>
        /// Removes the given value from the saved collection of those values.
        /// </summary>
        /// <typeparam name="T">The type of the value to remove.</typeparam>
        /// <param name="value">The value to remove.</param>
        /// <param name="fileName">The name of the file to remove the value from.</param>
        /// <param name="adapter">The adapter to use to serialize the values. Defaults to EntityJsonAdapter if none is provided.</param>
        /// <returns>The removed value, or the default value if nothing was removed.</returns>
        public async Task<T> RemoveAsync<T>(T value, string fileName, IJsonAdapter<T>? adapter = null)
        {
            List<T> allValues = await this.LoadAsync(fileName, adapter);
            bool removed = allValues.Remove(value);
            await this.SaveAsync(allValues, fileName, adapter);

            return removed ? value : default;
        }

        /// <summary>
        /// Removes the given values from the saved collection of those values.
        /// </summary>
        /// <typeparam name="T">The type of the value to remove.</typeparam>
        /// <param name="values">The values to remove.</param>
        /// <param name="fileName">The name of the file to remove the values from.</param>
        /// <param name="adapter">The adapter to use to serialize the values. Defaults to EntityJsonAdapter if none is provided.</param>
        /// <returns>The removed values, or an empty collection if nothing was removed.</returns>
        public async Task<IEnumerable<T>> RemoveAsync<T>(IEnumerable<T> values, string fileName, IJsonAdapter<T>? adapter = null)
        {
            List<T> removedValues = new List<T>();
            List<T> allValues = await this.LoadAsync(fileName, adapter);
            foreach (T value in values)
            {
                if(allValues.Remove(value))
                {
                    removedValues.Add(value);
                }
            }
            await this.SaveAsync(allValues, fileName, adapter);

            IEnumerable<T> results = removedValues;
            return results;
        }
    }
}
