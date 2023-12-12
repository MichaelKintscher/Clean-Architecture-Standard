using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace CleanArchitectureStandard.Adapters.Json
{
    /// <summary>
    /// Contians methods to convert entities to/from JSON with shallow serialization.
    /// </summary>
    public static class EntityJsonAdapter<T>
    {
        private static readonly JsonSerializerOptions options = new JsonSerializerOptions()
        {
            Converters = { new TimeOnlyJsonConverter() }
        };

        /// <summary>
        /// Serializes an entity to JSON.
        /// </summary>
        /// <param name="entity">The entity to serialize.</param>
        /// <returns></returns>
        public static string Serialize(T entity)
        {
            // There is no entity to serialize.
            if (entity == null)
            {
                return string.Empty;
            }

            // Serialize the entity data.
            return JsonSerializer.Serialize(entity, EntityJsonAdapter<T>.options);
        }

        /// <summary>
        /// Deserializes and entity from JSON.
        /// </summary>
        /// <param name="json">The JSON to deserialize from.</param>
        /// <returns></returns>
        /// <exception cref="JsonException">Thrown if the given JSON is not in the correct format.</exception>
        public static T Deserialize(string json)
        {
            // Parse the entity data and add it to the list.
            T? entity = JsonSerializer.Deserialize<T>(json, EntityJsonAdapter<T>.options);
            if (entity == null)
            {
                throw new JsonException("JSON to deserialize " + typeof(T).Name + " from was not in the expected format.");
            }

            return entity;
        }

        /// <summary>
        /// Serializes a list of entities to JSON. The list is converted to a JSON
        /// array of JSON objects. The array itself is stored as a property on the
        /// root JSON object with the given property name.
        /// </summary>
        /// <param name="entities">The list of entities to serialize.</param>
        /// <param name="propertyName">The property name to give the serialized JSON list on the root JSON object.</param>
        /// <returns></returns>
        public static string SerializeList(List<T> entities, string propertyName = "values")
        {
            // There are no entities to serialize.
            if (entities == null ||
                entities.Count < 1)
            {
                return string.Empty;
            }

            // For each entity in the list...
            JsonArray entitiesJsonArray = new JsonArray();
            foreach (T entity in entities)
            {
                // Serialize the entity data and add it to the array.
                JsonDocument entityJson = JsonSerializer.SerializeToDocument(entity, EntityJsonAdapter<T>.options);
                entitiesJsonArray.Add(entityJson);
            }

            // Store the array in a json object.
            JsonObject jsonObject = new JsonObject();
            jsonObject.Add(propertyName, entitiesJsonArray);

            return jsonObject.ToJsonString();
        }

        /// <summary>
        /// Deserializes a list of entities from JSON.
        /// </summary>
        /// <param name="json">The JSON to deserialize from. The list of entities should be stored on a property of the root JSON object as a JSON array of JSON objects.</param>
        /// <param name="propertyName">The name of the property on the root JSON object the JSON array is stored on.</param>
        /// <returns></returns>
        /// <exception cref="JsonException">Thrown if the given JSON is not in the correct format.</exception>
        public static List<T> DeserializeList(string json, string propertyName = "values")
        {
            // Validate the JSON format.
            JsonNode? jsonObject = JsonNode.Parse(json);
            if (jsonObject == null)
            {
                throw new JsonException("JSON to deserialize " + typeof(T).Name + " list from was not in the expected format.");
            }

            JsonArray entityJsonArray;
            try
            {
                entityJsonArray = jsonObject[propertyName].AsArray();
            }
            catch (Exception ex)
            {
                throw new JsonException("Error was encountered when parsing the JSON.", ex);
            }

            // For each entity in the array...
            List<T> entities = new List<T>();
            foreach (var entityJsonValue in entityJsonArray)
            {
                // This is necessary because of the type iterated over in the JsonArray.
                string entityJson = entityJsonValue.ToJsonString();

                // Parse the entity data and add it to the list.
                T? entity = JsonSerializer.Deserialize<T>(entityJson, EntityJsonAdapter<T>.options);
                if (entity == null)
                {
                    throw new JsonException("JSON to deserialize " + typeof(T).Name + " from was not in the expected format.");
                }
                entities.Add(entity);
            }

            return entities;
        }
    }
}
