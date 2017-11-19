﻿using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Cache.Implementation;

namespace AzureFromTheTrenches.Commanding.Cache
{
    // ReSharper disable once InconsistentNaming
    public static class CacheCommandingDependencies
    {
        /// <summary>
        /// Sets up the cache with the default cache key provider that uses the command type, property names and property values to
        /// generate a hashable string
        /// </summary>
        /// <param name="resolver">The dependency resolver</param>
        /// <param name="options">Cache options</param>
        /// <returns>The dependency resolver</returns>
        public static ICommandingDependencyResolver UseCommandCache(this ICommandingDependencyResolver resolver,  params CacheOptions[] options)
        {
            return UseCommandCache(resolver, new PropertyCacheKeyProvider(new PropertyCacheKeyProviderCompiler(), new SimpleCacheKeyHash()), options);
        }

        /// <summary>
        /// Sets up the cache with the specified cache key provider
        /// </summary>
        /// <param name="resolver">The dependency resolver</param>
        /// <param name="cacheKeyProvider">Instance of a cache key provider</param>
        /// <param name="options">Cache options</param>
        /// <returns>The dependency resolver</returns>
        public static ICommandingDependencyResolver UseCommandCache(this ICommandingDependencyResolver resolver, ICacheKeyProvider cacheKeyProvider, params CacheOptions[] options)
        {
            ICacheOptionsProvider cacheOptionsProvider = new CacheOptionsProvider(options);
            resolver.RegisterInstance(cacheOptionsProvider);
            resolver.TypeMapping<ICachedCommandDispatcher, CachedCommandDispatcher>();
            resolver.RegisterInstance(cacheKeyProvider);
            
            return resolver;
        }
    }
}
