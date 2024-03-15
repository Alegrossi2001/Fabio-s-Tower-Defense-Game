using System;
using System.Collections.Generic;
using Dypsloom.RhythmTimeline.Core;
using Dypsloom.RhythmTimeline.Core.Notes;
using Dypsloom.RhythmTimeline.Core.Playables;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

namespace Dypsloom.RhythmTimeline.Editor
{
    [CustomEditor(typeof(RhythmTrack), true)]
    public class RhythmTrackInspector : UIElementsInspector
    {
        private const string AUTO_FILL_NOTE_DEFINITION_GUID_KEY = "AUTO_FILL_NOTE_DEFINITION_GUID_KEY";

        [Serializable]
        public enum AddOption
        {
            ClearPreviousNotes,
            AddOnTop,
            ReplaceOnOverlap,
            DontReplaceOnOverlap
        }
        
        protected override List<string> propertiesToExclude => new List<string>() { m_ItemPropertyName };

        protected const string m_ItemPropertyName = "m_StartItem";

        protected RhythmTrack m_RhythmTrack;

        protected EnumField m_AddOptionOption;
        protected Button m_SpawnNotesButton;
        protected Button m_ClearSectionButton;
        protected Button m_ClearClipsButton;
        protected Button m_RefreshTimeline;
        protected ObjectField m_AutoFillNoteDefinitionField;
        protected FloatField m_AutofillBpmField;
        protected FloatField m_StartTimeField;
        protected FloatField m_EndTimeField;
        protected FloatField m_AutofillNoteSpacingField;
        protected FloatField m_AutofillNoteLengthField;
        protected VisualElement m_RhythmTrackContainer;

        protected List<TimelineClip> m_CachedClipsInRange;
 
        public override VisualElement CreateInspectorGUI()
        {
            m_CachedClipsInRange = new List<TimelineClip>();
            
            var container = new VisualElement();
 
            m_RhythmTrack = target as RhythmTrack;
 
            var UIElementFields = CreateUIElementInspectorGUI(serializedObject, propertiesToExclude);
 
            m_RhythmTrackContainer = new VisualElement();

            m_AutoFillNoteDefinitionField = new ObjectField("Note Definition");
            m_AutoFillNoteDefinitionField.objectType = typeof(NoteDefinition);
            m_AutoFillNoteDefinitionField.value = LoadNoteDefinition();
            m_AutoFillNoteDefinitionField.RegisterValueChangedCallback(evt =>
            {
                SaveNoteDefinition(evt.newValue as NoteDefinition);
            });
            
            m_AutofillBpmField = new FloatField("BPM");
            m_AutofillBpmField.value = (m_RhythmTrack.timelineAsset as RhythmTimelineAsset)?.Bpm ?? 120;
 
            m_StartTimeField = new FloatField("Start Time");
            m_StartTimeField.value = 0;
            m_EndTimeField = new FloatField("End Time");
            m_EndTimeField.value = (float)m_RhythmTrack.timelineAsset.duration;
 
            m_AutofillNoteSpacingField = new FloatField("Note Spacing");
            m_AutofillNoteSpacingField.tooltip = "Note Spacing in Beat";
            m_AutofillNoteSpacingField.value = 1;
            
            m_AutofillNoteLengthField = new FloatField("Note Length");
            m_AutofillNoteLengthField.tooltip = "Note Length in Beat";
            m_AutofillNoteLengthField.value = 1;

            m_AddOptionOption = new EnumField("Add Option",AddOption.ClearPreviousNotes);

            m_SpawnNotesButton = new Button();
            m_SpawnNotesButton.text = "Autofill Track Notes";
            m_SpawnNotesButton.clickable.clicked += AutofillTrack;
 
            m_ClearSectionButton = new Button();
            m_ClearSectionButton.text = "Clear Section";
            m_ClearSectionButton.clickable.clicked += ClearSection;
            
            m_ClearClipsButton = new Button();
            m_ClearClipsButton.text = "Clear All Notes";
            m_ClearClipsButton.clickable.clicked += ClearAllTrack;
 
            m_RefreshTimeline = new Button();
            m_RefreshTimeline.text = "Refresh Editor";
            m_RefreshTimeline.clickable.clicked += RefreshTimeline;
 
            m_RhythmTrackContainer.Add(m_AutoFillNoteDefinitionField);
            m_RhythmTrackContainer.Add(m_AutofillBpmField);
            m_RhythmTrackContainer.Add(m_AutofillNoteLengthField);
            m_RhythmTrackContainer.Add(m_AutofillNoteSpacingField);
            m_RhythmTrackContainer.Add(m_StartTimeField);
            m_RhythmTrackContainer.Add(m_EndTimeField);
            m_RhythmTrackContainer.Add(m_AddOptionOption);
            m_RhythmTrackContainer.Add(m_SpawnNotesButton);
            m_RhythmTrackContainer.Add(m_ClearSectionButton);
            m_RhythmTrackContainer.Add(m_ClearClipsButton);
            m_RhythmTrackContainer.Add(m_RefreshTimeline);
            var addRhythmFoldout = new Foldout();
            addRhythmFoldout.text = "Rhythm Track Autofill Tools";
            addRhythmFoldout.Add(m_RhythmTrackContainer);
 
            container.Add(UIElementFields);
            container.Add(addRhythmFoldout);
 
 
            return container;
        }

        private NoteDefinition LoadNoteDefinition()
        {
            var guid = EditorPrefs.GetString(AUTO_FILL_NOTE_DEFINITION_GUID_KEY, "");
            if (string.IsNullOrEmpty(guid)) {
                return null; 
            }
            var guidToAssetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(guidToAssetPath)) {
                return null; 
            }
            
            return AssetDatabase.LoadAssetAtPath<NoteDefinition>(guidToAssetPath);
        }
        
        private void SaveNoteDefinition(NoteDefinition noteDefinition)
        {
            if (noteDefinition == null) {
                EditorPrefs.SetString(AUTO_FILL_NOTE_DEFINITION_GUID_KEY, "");
                return;
            }

            var assetPath = AssetDatabase.GetAssetPath(noteDefinition);
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            EditorPrefs.SetString(AUTO_FILL_NOTE_DEFINITION_GUID_KEY, guid);
        }

        private void AutofillTrack()
        {
            var step = 60d / m_AutofillBpmField.value;
            var startTime = m_StartTimeField.value;
            var endTime = m_EndTimeField.value;
            float noteSpacing = m_AutofillNoteSpacingField.value;
            float noteLength = m_AutofillNoteLengthField.value;

            if (startTime < 0) {
                startTime = 0;
            }
            if (startTime > endTime)
            {
                Debug.LogError(m_EndTimeField.text + " cannot be smaller than the start time.");
                return;
            }
            if (noteSpacing < 0)
            {
                Debug.LogError(m_AutofillNoteSpacingField.text + " cannot be negative");
                return;
            }
            if (noteLength < 1)
            {
                Debug.LogError(m_AutofillNoteSpacingField.text + " cannot be 0 or negative");
                return;
            }

            switch ((AddOption)m_AddOptionOption.value) {
                case AddOption.AddOnTop:
                    AddNotes(startTime, endTime, step, noteLength, noteSpacing);
                    break;
                case AddOption.ClearPreviousNotes:
                    ClearSection();
                    AddNotes(startTime, endTime, step, noteLength, noteSpacing);
                    break;
                case AddOption.ReplaceOnOverlap:
                    AddNotesOverlap(startTime, endTime, step, noteLength, noteSpacing, true);
                    break;
                case AddOption.DontReplaceOnOverlap:
                    AddNotesOverlap(startTime, endTime, step, noteLength, noteSpacing, false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            RefreshTimeline();
        }

        protected virtual void AddNotes(float startTime, float endTime, double step, float noteLength, float noteSpacing)
        {
            var stepCount = (endTime - startTime) / (step * (noteSpacing + noteLength));
            
            //fill notes by beat spacing
            for (double i = 0; i < stepCount; i++) {
                var clipStart = (step * i * (noteSpacing + noteLength)) + startTime;
                var clipDuration = noteLength * step;
                
                var clip = m_RhythmTrack.CreateClip<RhythmClip>();
                var clipAsset = clip.asset as RhythmClip;
                clipAsset.RhythmPlayableBehaviour.SetNoteDefinition(m_AutoFillNoteDefinitionField.value as NoteDefinition);

                clip.start = clipStart;
                clip.duration = clipDuration;
            }
        }
        
        protected virtual void AddNotesOverlap(float startTime, float endTime, double step, float noteLength, float noteSpacing, bool replace)
        {
            var stepCount = (endTime - startTime) / (step * (noteSpacing + noteLength));
            
            //fill notes by beat spacing
            for (int i = 0; i < stepCount; i++) {
                var clipStart = (step * i * (noteSpacing + noteLength)) + startTime;
                var clipDuration = noteLength * step;

                var clips = GetClipsInRange(clipStart, clipStart + clipDuration);

                if (clips.Count != 0) {
                    if (replace == false) {
                        continue;
                    }
                    
                    for (int j = 0; j < clips.Count; j++) {
                        m_RhythmTrack.timelineAsset.DeleteClip(clips[j]);
                    }
                }
                
                var clip = m_RhythmTrack.CreateClip<RhythmClip>();
                var clipAsset = clip.asset as RhythmClip;
                clipAsset.RhythmPlayableBehaviour.SetNoteDefinition(m_AutoFillNoteDefinitionField.value as NoteDefinition);

                clip.start = clipStart;
                clip.duration = clipDuration;
            }
        }

        private List<TimelineClip> GetClipsInRange(double start, double end)
        {
            m_CachedClipsInRange.Clear();
            
            var clips = m_RhythmTrack.GetClips();
 
            foreach (var clip in clips)
            {
                if (clip.end < start || clip.start > end) {
                    continue;
                }
                m_CachedClipsInRange.Add(clip);
            }

            return m_CachedClipsInRange;
        }

        private void RefreshTimeline()
        {
            TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved | RefreshReason.WindowNeedsRedraw);
        }
 
        private void ClearAllTrack()
        {
            var clips = m_RhythmTrack.GetClips();
 
            foreach (var clip in clips)
            {
                m_RhythmTrack.timelineAsset.DeleteClip(clip);
            }
 
            RefreshTimeline();
        }
        
        private void ClearSection()
        {
            ClearSection(m_StartTimeField.value, m_EndTimeField.value);
            
        }
        
        private void ClearSection(float startTime, float endTime)
        {
            var clips = m_RhythmTrack.GetClips();
 
            foreach (var clip in clips)
            {
                if (clip.end < startTime || clip.start > endTime) {
                    return;
                }
                m_RhythmTrack.timelineAsset.DeleteClip(clip);
            }
 
            RefreshTimeline();
        }
    }
}