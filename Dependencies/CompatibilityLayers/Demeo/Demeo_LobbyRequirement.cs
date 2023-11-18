using System;

namespace MonkiiLoader
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class Demeo_LobbyRequirement : Attribute
    {
        public Demeo_LobbyRequirement() { }
    }
}
