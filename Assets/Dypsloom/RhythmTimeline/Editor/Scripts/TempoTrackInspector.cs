using Dypsloom.RhythmTimeline.Core;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Dypsloom.RhythmTimeline.Editor
{
    using Dypsloom.RhythmTimeline.Core.Playables;

    [CustomEditor(typeof(TempoTrack), true)]
    public class TempoTrackInspector : UIElementsInspector
    {
        protected override List<string> propertiesToExclude => new List<string>() { m_ItemPropertyName };

        protected const string m_ItemPropertyName = "m_StartItem";

        protected TempoTrack m_TempoTrack;
    
        protected Button m_SetTempoButton;
        protected Button m_ClearMarkerButton;
        protected FloatField m_BpmField;
        protected FloatField m_OffsetField;
        protected FloatField m_DurationField;
        protected Toggle m_WithOnBeatMarker;
        protected Toggle m_WithOffBeatMarker;
        protected VisualElement m_TempoContainer;

        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();

            m_TempoTrack = target as TempoTrack;

            var UIElementFields = CreateUIElementInspectorGUI(serializedObject, propertiesToExclude);
        
            m_TempoContainer = new VisualElement();
        
            m_BpmField = new FloatField("BPM");
            m_BpmField.value = (m_TempoTrack.timelineAsset as RhythmTimelineAsset)?.Bpm ?? 120;
            //m_BpmField.RegisterValueChangedCallback(c => {  });
        
            m_OffsetField = new FloatField("Offset");
            m_OffsetField.value = 0;
            m_DurationField = new FloatField("Duration");
            m_DurationField.value = (float)m_TempoTrack.timelineAsset.duration;
            //m_DurationField.RegisterValueChangedCallback(c => {  });
            m_WithOnBeatMarker = new Toggle("With On Beat Marker");
            m_WithOnBeatMarker.value = true;
            m_WithOffBeatMarker = new Toggle("With Off Beat Marker");
            m_WithOffBeatMarker.value = true;
        
            m_SetTempoButton = new Button();
            m_SetTempoButton.text = "SetTempo";
            m_SetTempoButton.clickable.clicked += SetTempo;
        
            m_ClearMarkerButton = new Button();
            m_ClearMarkerButton.text = "Clear Markers";
            m_ClearMarkerButton.clickable.clicked += ClearMarkers;

            m_TempoContainer.Add(m_BpmField);
            m_TempoContainer.Add(m_OffsetField);
            m_TempoContainer.Add(m_DurationField);
            m_TempoContainer.Add(m_WithOnBeatMarker);
            m_TempoContainer.Add(m_WithOffBeatMarker);
            m_TempoContainer.Add(m_SetTempoButton);
            m_TempoContainer.Add(m_ClearMarkerButton);
            
            var addTempoFoldout = new Foldout();
            addTempoFoldout.text = "Add Tempo";
            addTempoFoldout.Add(m_TempoContainer);

            container.Add(UIElementFields);
            container.Add(addTempoFoldout);


            return container;
        }

        private void SetTempo()
        {
            var step = 60d / m_BpmField.value;
            var duration = m_DurationField.value;
            var stepCount = duration / step;

            if (m_WithOnBeatMarker.value) {
                for (double i = 0; i < stepCount; i ++) {
                    var tempoMarker = m_TempoTrack.CreateMarker<TempoMarker>((step*i) + m_OffsetField.value);
                    tempoMarker.name = "BPM On Beat Marker";
                    tempoMarker.ID = m_TempoTrack.OnBeat?.ID ?? 0;
                }
            }

            var halfStep = step / 2f;
        
            if (m_WithOffBeatMarker.value) {
                for (double i = 0; i < stepCount; i ++) {
                    var tempoMarker = m_TempoTrack.CreateMarker<TempoMarker>((step*i)- halfStep + m_OffsetField.value);
                    tempoMarker.name = "BPM Off Beat Marker";
                    tempoMarker.ID = m_TempoTrack.OffBeat?.ID ?? 1;
                }
            }

            TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved | RefreshReason.WindowNeedsRedraw);
        }

        private void ClearMarkers()
        {
            var markers = m_TempoTrack.GetMarkers();
            foreach (var marker in markers) {
                m_TempoTrack.DeleteMarker(marker);
            }
        }
    }
}