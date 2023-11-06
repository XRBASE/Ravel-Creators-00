using System;

namespace Base.Ravel.FeatureFlags
{
    //these are the names of feature flags. In order to use them, the name should be added in here,
    //otherwise result cannot be added to the user's flag values
    //scores '-' in the backend data are replaced with underscores '_', as scores are not possible in enum values.
    [Flags]
    public enum FlagType
    {
        none = 0,
        video_streaming = 1<<0,
    }
}
