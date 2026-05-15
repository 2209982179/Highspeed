using highspeed.framework.Models;

namespace highspeed.framework.Common
{
    public class Reposhell : Repository
    {
        public static Reposhell Instance(SessionInfo session)
        {
            return Instance<Reposhell>(session);
        } 
    }

    public static class SDKReposhellExtension
    {
        public static Reposhell ToSDKReposhell(this Repository repo)
        {
            return Reposhell.Instance(repo.Session);
        }
    }
}
