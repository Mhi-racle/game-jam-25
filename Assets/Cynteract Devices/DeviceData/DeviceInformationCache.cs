using System;
using System.Collections.Generic;
using System.IO;
using Connector.Messages;
using Newtonsoft.Json;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Cynteract.InputDevices
{
    public class DeviceInformationCache
    {
        // The Application.persistentDataPath property must be accessed on the main thread.
        public static string JsonPath { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitializeJsonPath()
        {
            JsonPath = Path.Combine(Application.persistentDataPath, "DeviceInformationCache.json");
        }

        private Dictionary<string, Information> cache = null;
        private readonly object fileLock = new();

        private void InitCache()
        {
            // load cache
            if (cache == null)
            {
                try
                {
                    lock (fileLock)
                    {
                        string json = File.ReadAllText(JsonPath);
                        // Confirm the json doesn't contain a new key. Otherwise the protocol was updated.
                        var settings = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error, };
                        cache = JsonConvert.DeserializeObject<Dictionary<string, Information>>(json, settings);
                        // Confirm all keys are present in the json. Otherwise the protocol was updated.
                        // This could be implemented with the "required" keyword, which needs C# 11. Unity 2022.3 uses C# 9, though.
                        foreach (var entry in cache.Values)
                        {
                            foreach (var prop in entry.GetType().GetFields())
                            {
                                if (prop.GetValue(entry) == null)
                                    throw new JsonSerializationException($"Property '{prop.Name}' in cache entry cannot be null.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to load JSON data from {JsonPath}: {ex.Message}");
                    cache = new();
                }
            }
        }

        public void Save(string deviceId, Information data)
        {
            InitCache();
            cache[deviceId] = data;

            // save cache
            try
            {
                lock (fileLock)
                {
                    string json = JsonConvert.SerializeObject(cache, Formatting.Indented);
                    File.WriteAllText(JsonPath, json);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save JSON data to {JsonPath}: {ex.Message}");
            }
        }

        public Information Load(string deviceId)
        {
            InitCache();
            return cache.GetValueOrDefault(deviceId, null);
        }
    }
}
