using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyAI))]
public class EnemyAIEditor : Editor
{
    void OnSceneGUI()
    {
        var ai = (EnemyAI)target;
        if (ai.patrolPoints == null) return;
        Handles.color = new Color(0.2f, 0.8f, 1f, 0.4f);
        Handles.DrawWireDisc(ai.transform.position, Vector3.forward, ai.chaseRange);
        Handles.color = Color.yellow;
        for (int i = 0; i < ai.patrolPoints.Length; i++)
        {
            Vector3 p = ai.patrolPoints[i].position;
            Vector3 np = ai.patrolPoints[(i + 1) % ai.patrolPoints.Length].position;
            Handles.SphereHandleCap(0, p, Quaternion.identity, 0.1f, EventType.Repaint);
            Handles.DrawLine(p, np);
            EditorGUI.BeginChangeCheck();
            Vector3 newP = Handles.PositionHandle(p, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(ai, "Move Patrol Point");
                ai.patrolPoints[i].position = newP;
                EditorUtility.SetDirty(ai);
            }
            Handles.Label(p + Vector3.up * 0.2f, "Wait " + ai.patrolPoints[i].waitTime + "s");
        }
    }
}
