using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Core.Adapters.Json
{
    public interface IJsonAdapter<T>
    {
        /// <summary>
        /// Serializes an entity to JSON.
        /// </summary>
        /// <param name="entity">The entity to serialize.</param>
        /// <returns></returns>
        public string Serialize(T entity);

        /// <summary>
        /// Deserializes and entity from JSON.
        /// </summary>
        /// <param name="json">The JSON to deserialize from.</param>
        /// <returns></returns>
        /// <exception cref="JsonException">Thrown if the given JSON is not in the correct format.</exception>
        public T Deserialize(string json);

        /// <summary>
        /// Serializes a list of entities to JSON. The list is converted to a JSON
        /// array of JSON objects. The array itself is stored as a property on the
        /// root JSON object with the given property name.
        /// </summary>
        /// <param name="entities">The list of entities to serialize.</param>
        /// <param name="propertyName">The property name to give the serialized JSON list on the root JSON object.</param>
        /// <returns></returns>
        public string SerializeList(List<T> entities, string propertyName = "values");

        /// <summary>
        /// Deserializes a list of entities from JSON.
        /// </summary>
        /// <param name="json">The JSON to deserialize from. The list of entities should be stored on a property of the root JSON object as a JSON array of JSON objects.</param>
        /// <param name="propertyName">The name of the property on the root JSON object the JSON array is stored on.</param>
        /// <returns></returns>
        /// <exception cref="JsonException">Thrown if the given JSON is not in the correct format.</exception>
        public List<T> DeserializeList(string json, string propertyName = "values");
    }
}
