using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class AddScriptToButton : Editor
{
    [MenuItem("Tools/버튼 스크립트 일괄 추가")]
    private static void AddScriptToAllSceneButtons()
    {
        Button[] allBtn = Object.FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        if (allBtn.Length == 0)
        {
            Utils.LogWarning("씬에서 Button 컴포넌트를 찾지 못했습니다.");
            return;
        }

        int addedCount = 0;

        foreach (Button btn in allBtn)
        {
            GameObject obj = btn.gameObject;

            if (obj.GetComponent<UIButtonSound>() == null)
            {
                Undo.AddComponent<UIButtonSound>(obj);
                addedCount++;
            }
        }

        if (addedCount > 0)
        {
            EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }

        Utils.Log($"{allBtn.Length}개의 버튼 중, 스크립트가 없던 {addedCount}개의 버튼에 스크립트를 추가했습니다.");
    }
}
