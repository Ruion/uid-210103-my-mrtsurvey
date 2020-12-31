using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ComponentTaskInvoker : MonoBehaviour
{
    protected bool isDebugging;

    public UnityEvent autoTasks;

    public void TriggerButtonClick(Button btn)
    {
        VerifyDebugMode();

        if (!isDebugging) return;

        btn.onClick.Invoke();
    }

    public void ExecuteAutoTasks()
    {
        VerifyDebugMode();

        if (!isDebugging) return;

        if (autoTasks.GetPersistentEventCount() > 0) autoTasks.Invoke();
    }

    protected void VerifyDebugMode()
    {
        isDebugging = FindObjectOfType<GameSettingEntity>().gameSettings.debugMode;
       
    }
}
