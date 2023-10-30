using System;

namespace Base.Ravel.Modules
{
    //enum listing all modules, usable as flags, does need to be updated manually just like permissions

    [Flags]
    public enum ModuleFlags
    {
        None          = 0,
        Event         = 1<<0,
        HHC           = 1<<1,
        Moderation    = 1<<2,
        Polling       = 1<<3,
        Presenting    = 1<<4,
        Waterschappen = 1<<5,
    }
}