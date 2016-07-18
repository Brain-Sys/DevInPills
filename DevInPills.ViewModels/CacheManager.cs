using DevInPills.ViewModels.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DevInPills.ViewModels
{
    /// <summary>
    /// This class implements a cross-platform cache manager
    /// It load or save any data from the file system
    /// </summary>
    public class CacheManager
    {
        private Dictionary<Type, Timer> internalTimers;
        private JsonSerializer serializer;

        // Some functions needed by the cache manager depends
        // on the operating system. Using these delegates the developer
        // can inject some external functions:
        // - IsCacheAvailableAsync : check whether a cache file exists
        // - GetStreamForReadAsync : get the stream to read from a file
        // - GetStreamForWriteAsync : get the stream to write on a file
        public Func<string, Task<bool>> IsCacheAvailableAsync { get; set; }
        public Func<string, Task<Stream>> GetStreamForReadAsync { get; set; }
        public Func<string, Task<Stream>> GetStreamForWriteAsync { get; set; }
        public TimeSpan CacheDuration { get; private set; }
        public event EventHandler CacheExpired;

        public CacheManager()
        {
            internalTimers = new Dictionary<Type, Timer>();
            serializer = new JsonSerializer() { TypeNameHandling = TypeNameHandling.All };
            this.CacheDuration = TimeSpan.FromMinutes(15);
        }

        public CacheManager(TimeSpan cacheDuration) : this()
        {
            this.CacheDuration = cacheDuration;
        }

        /// <summary>
        /// Save data on a file
        /// This method is generic, so can be used for any type of data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data to be saved</param>
        /// <param name="filename">Output filename</param>
        /// <returns></returns>
        public Task SaveAsync<T>(T data, string filename)
        {
            return Task.Run(async () =>
            {
                var stream = await this.GetStreamForWriteAsync(filename);

                using (JsonTextWriter jsonwriter = new JsonTextWriter(new StreamWriter(stream)))
                {
                    serializer.Serialize(jsonwriter, data);
                }

                stream.Dispose();
                stream = null;

                startNewTimer(typeof(T));
            });
        }

        /// <summary>
        /// Load data from a file
        /// This method is generic, so can be used for any type of data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename">Input filename</param>
        /// <returns>The data loaded from the file</returns>
        public Task<T> LoadAsync<T>(string filename)
        {
            return Task.Run(async () =>
            {
                T result;
                var stream = await this.GetStreamForReadAsync(filename);

                using (JsonTextReader jsonreader = new JsonTextReader(new StreamReader(stream)))
                {
                    object obj = serializer.Deserialize(jsonreader);
                    result = (T)obj;
                }

                stream.Dispose();
                stream = null;

                return result;
            });
        }

        /// <summary>
        /// Start a new timer to rise the event CacheExpired
        /// </summary>
        /// <param name="type"></param>
        private void startNewTimer(Type type)
        {
            if (internalTimers.ContainsKey(type))
            {
                internalTimers[type].Cancel();
                internalTimers[type].Dispose();
                internalTimers.Remove(type);
            }

            internalTimers.Add(type, new Timer(cacheExpiredTimer, type, this.CacheDuration, TimeSpan.Zero));
        }

        /// <summary>
        /// Rise the event CacheExpired
        /// </summary>
        /// <param name="obj"></param>
        private void cacheExpiredTimer(object obj)
        {
            this.CacheExpired?.Invoke(this, EventArgs.Empty);
        }
    }
}