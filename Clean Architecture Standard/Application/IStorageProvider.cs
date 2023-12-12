using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitectureStandard.Application
{
    public interface IStorageProvider<T>
    {
        /// <summary>
        /// Saves a list of values of the given type.
        /// </summary>
        /// <typeparam name="T">The type of values to save.</typeparam>
        /// <param name="values">The values to save.</param>
        /// <param name="fileName">The name of the file to save the values to.</param>
        /// <returns></returns>
        public Task SaveAsync<T>(List<T> values, string fileName);

        /// <summary>
        /// Loads a list of values of the given type.
        /// </summary>
        /// <typeparam name="T">The type of values to load.</typeparam>
        /// <param name="fileName">The name of the file to load the values from.</param>
        /// <returns></returns>
        public Task<List<T>> LoadAsync<T>(string fileName);

        /// <summary>
        /// Adds the given value to the saved collection of that value.
        /// </summary>
        /// <typeparam name="T">The type of the value to add.</typeparam>
        /// <param name="value">The value to add.</param>
        /// <param name="fileName">The name of the file to add the value to.</param>
        /// <returns></returns>
        public Task AddAsync<T>(T value, string fileName);
    }
}
