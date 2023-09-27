namespace Bamboo.Configuration.Remote
{
    internal static class RemoteManager
    {
        public static void Fetch()
        {
            // Git source
            GitRemoteManager.Fetch();
        }
    }
}
