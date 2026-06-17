using System.Collections.Generic;
using UnityEngine;

public class HitManager : MonoBehaviour
{
    public static List<Note> Notes = new List<Note>();

    // 判定范围
    public static float hitRange = 3f;

    public static void Register(Note note)
    {
        if (note == null)
            return;

        if (!Notes.Contains(note))
            Notes.Add(note);
    }

    public static void Unregister(Note note)
    {
        if (note == null)
            return;

        Notes.Remove(note);
    }

    public static Note FindTarget(NoteColor color)
    {
        Note target = null;

        float closest = Mathf.Infinity;

        foreach (Note note in Notes)
        {
            if (note == null)
                continue;

            if (!note.IsActive())
                continue;

            if (note.noteColor != color)
                continue;

            float distance = note.GetDistanceToJudgeLine();

            // 不在判定区域
            // 不允许击打
            if (distance > hitRange)
                continue;

            if (distance < closest)
            {
                closest = distance;
                target = note;
            }
        }

        return target;
    }
}