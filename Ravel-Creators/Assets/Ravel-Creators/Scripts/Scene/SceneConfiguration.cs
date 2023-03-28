using UnityEngine;

public class SceneConfiguration : MonoBehaviour
{
    public EnvironmentSO environmentSO;

    public bool EnvironmentCanBuild() {
        return !environmentSO.environment.published;
    }
}
