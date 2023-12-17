using CleanArchitecture.Core.Adapters.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Core.Application
{
    public interface IStorageProvider<T>
    {
        /// <summary>
        /// Saves a list of values of the given type.
        /// </summary>
        /// <param name="values">The values to save.</param>
        /// <param name="fileName">The name of the file to save the values to.</param>
        /// <param name="adapter">The adapter to use to serialize the values. Defaults to EntityJsonAdapter if none is provided.</param>
        /// <returns></returns>
        public Task SaveAsync(List<T> values, string fileName, IJsonAdapter<T>? adapter = null);

        /// <summary>
        /// Loads a list of values of the given type.
        /// </summary>
        /// <param name="fileName">The name of the file to load the values from.</param>
        /// <param name="adapter">The adapter to use to serialize the values. Defaults to EntityJsonAdapter if none is provided.</param>
        /// <returns></returns>
        public Task<List<T>> LoadAsync(string fileName, IJsonAdapter<T>? adapter = null);

        /// <summary>
        /// Adds the given value to the saved collection of that value.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <param name="fileName">The name of the file to add the value to.</param>
        /// <param name="adapter">The adapter to use to serialize the values. Defaults to EntityJsonAdapter if none is provided.</param>
        /// <returns></returns>
        public Task AddAsync(T value, string fileName, IJsonAdapter<T>? adapter = null);

        /// <summary>
        /// Adds the given values to the saved collection of those values.
        /// </summary>
        /// <param name="values">The values to add.</param>
        /// <param name="fileName">The name of the file to add the value to.</param>
        /// <param name="adapter">The adapter to use to serialize the values. Defaults to EntityJsonAdapter if none is provided.</param>
        /// <returns></returns>
        public Task AddAsync(IEnumerable<T> values, string fileName, IJsonAdapter<T>? adapter = null);

        /// <summary>
        /// Removes the given value from the saved collection of those values.
        /// </summary>
        /// <param name="value">The value to remove.</param>
        /// <param name="fileName">The name of the file to remove the value from.</param>
        /// <param name="adapter">The adapter to use to serialize the values. Defaults to EntityJsonAdapter if none is provided.</param>
        /// <returns>The removed value, or the default value if nothing was removed.</returns>
        public Task<T> RemoveAsync(T value, string fileName, IJsonAdapter<T>? adapter = null);

        /// <summary>
        /// Removes the given values from the saved collection of those values.
        /// </summary>
        /// <param name="values">The values to remove.</param>
        /// <param name="fileName">The name of the file to remove the values from.</param>
        /// <param name="adapter">The adapter to use to serialize the values. Defaults to EntityJsonAdapter if none is provided.</param>
        /// <returns>The removed values, or an empty collection if nothing was removed.</returns>
        public Task<IEnumerable<T>> RemoveAsync(IEnumerable<T> values, string fileName, IJsonAdapter<T>? adapter = null);
    }
}
