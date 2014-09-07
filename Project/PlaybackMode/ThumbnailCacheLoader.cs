﻿using Kazyx.WPMMM.DataModel;
using Kazyx.WPPMM.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;

namespace Kazyx.WPMMM.PlaybackMode
{
    public class ThumbnailCacheLoader
    {
        private const string CACHE_ROOT = "/thumb_cache";

        private ThumbnailCacheLoader()
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!store.DirectoryExists(CACHE_ROOT))
                {
                    store.CreateDirectory(CACHE_ROOT);
                }
            }
        }

        private static readonly ThumbnailCacheLoader instance = new ThumbnailCacheLoader();

        public static ThumbnailCacheLoader INSTANCE
        {
            get { return instance; }
        }

        /// <summary>
        /// Asynchronously download thumbnail image and return local cache path.
        /// </summary>
        /// <param name="uuid">UUID of the target device.</param>
        /// <param name="content">Source of thumbnail image.</param>
        /// <returns>Path to local thumbnail cache.</returns>
        public async Task<string> GetCachePath(string uuid, ContentInfo content)
        {
            var uri = new Uri(content.ThumbnailUrl);
            var directory = CACHE_ROOT + "/" + uuid;
            var filename = directory + "/" + Path.GetFileName(uri.LocalPath);

            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!store.DirectoryExists(directory))
                {
                    store.CreateDirectory(directory);
                }

                lock (this)
                {
                    if (store.FileExists(filename))
                    {
                        Debug.WriteLine("Existing thumbnail cache: " + filename);
                        return filename;
                    }
                }
            }

            using (var stream = await Downloader.GetResponseStreamAsync(uri))
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    lock (this)
                    {
                        if (!store.FileExists(filename))
                        {
                            using (var dst = store.CreateFile(filename))
                            {
                                stream.CopyTo(dst);
                            }
                        }
                    }
                    Debug.WriteLine("New thumbnail cache: " + filename);
                    return filename;
                }
            }
        }
    }
}
