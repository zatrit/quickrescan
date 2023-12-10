using BepInEx;
using UnityEngine.InputSystem;
using static QuickRescan.MyPluginInfo;
using static System.Reflection.BindingFlags;

namespace QuickRescan;

[BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin {
    void Awake() {
        var desc = "See https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.InputControlPath.html";
        var rescanKeyName = Config.Bind("General", "Rescan key", "<Mouse>/forwardButton", desc);

        var rescanKey = new InputAction(binding: rescanKeyName.BoxedValue as string);

        rescanKey.performed += ctx => {
            var hud = HUDManager.Instance;

            if (hud is null) {
                return;
            }

            var type = hud.GetType();
            var modifier = NonPublic | Instance;

            var disable = type.GetMethod("DisableAllScanElements", modifier);
            disable.Invoke(hud, []);

            var timer = type.GetField("playerPingingScan", modifier);
            timer.SetValue(hud, -1f);

            var perform = type.GetMethod("PingScan_performed", modifier);
            perform.Invoke(hud, [ctx]);
        };
        rescanKey.Enable();
    }
}