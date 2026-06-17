using System.Collections.Generic;
using UnityEngine;

public class HitManager : MonoBehaviour
{
    public static List<Note> Notes = new List<Note>();

    public static float hitRange = 3f;


    public static void Register(Note note)
    {
        if (note != null && !Notes.Contains(note))
            Notes.Add(note);
    }


    public static void Unregister(Note note)
    {
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


            if (!note.IsInHitRange())
                continue;



            float distance =
                note.GetDistanceToJudgeLine();


            if (distance < closest)
            {
                closest = distance;
                target = note;
            }
        }


        return target;
    }
}