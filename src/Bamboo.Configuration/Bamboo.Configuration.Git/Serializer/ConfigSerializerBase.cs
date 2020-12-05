namespace Bamboo.Configuration.Git.Serializer
{
    /// <summary>
    /// file configuration serializer base class
    /// </summary>
    internal abstract class ConfigSerializerBase
    {
        /// <summary>
        /// get instance from config file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileFullPath">config file full path</param>
        /// <returns></returns>
        internal abstract T Deserialize<T>(string fileFullPath) where T : class, new();
    }
}
