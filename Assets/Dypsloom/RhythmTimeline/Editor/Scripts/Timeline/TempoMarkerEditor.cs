
namespace Dypsloom.RhythmTimeline.Editor.TimeLine
{
    using Dypsloom.RhythmTimeline.Core.Playables;
    using System;
    using UnityEditor;
    using UnityEditor.Timeline;
    using UnityEngine;
    using UnityEngine.Timeline;

    [CustomTimelineEditor(typeof(TempoMarker))]
    public class TempoMarkerEditor : MarkerEditor
    {
        public static readonly string s_DefaultOnBeatMarkerSettings = "Assets/Dypsloom/RhythmTimeline/Editor/DefaultOnBeatMarkerSettings.asset";
        public static readonly string s_DefaultOffBeatMarkerSettings = "Assets/Dypsloom/RhythmTimeline/Editor/DefaultOffBeatMarkerSettings.asset";
        
        public override void OnCreate(IMarker marker, IMarker clonedFrom)
        {
            base.OnCreate(marker, clonedFrom);

            (marker as TempoMarker).Copy(clonedFrom as TempoMarker);
        }

        public override MarkerDrawOptions GetMarkerOptions(IMarker marker)
        {
            return base.GetMarkerOptions(marker);
        }

        public override void DrawOverlay(IMarker marker, MarkerUIStates uiState, MarkerOverlayRegion region)
        {
            var tempoMarker = marker as TempoMarker;
            var tempoTrack = tempoMarker.parent as TempoTrack;

            if (tempoTrack == null) {
                Debug.LogWarning("Tempo Markers must be used on tempo tracks.");
                base.DrawOverlay(marker, uiState, region);
                return;
            }
            var markerEditorSettings=tempoTrack.GetMarkerEditorSettings(tempoMarker);
            if (markerEditorSettings == null) {
                if (tempoMarker.ID == 0) {
                    markerEditorSettings = (TempoMarkerEditorSettings)AssetDatabase.LoadAssetAtPath (s_DefaultOnBeatMarkerSettings, typeof(TempoMarkerEditorSettings));
                }else if (tempoMarker.ID == 1) {
                    markerEditorSettings = (TempoMarkerEditorSettings)AssetDatabase.LoadAssetAtPath (s_DefaultOffBeatMarkerSettings, typeof(TempoMarkerEditorSettings));
                } 
                
                if(markerEditorSettings == null) {
                    Debug.LogWarning("The custom Tempo Marker Editor cannot find the Marker Editor Settings.");
                    base.DrawOverlay(marker, uiState, region);
                    return;
                }
            }
            
            var iconsSize = new Vector2(18,18);

            var markerRegion = new Rect(
                region.markerRegion.center.x-iconsSize.x/2,
                region.markerRegion.position.y,
                iconsSize.x,
                iconsSize.y);
            markerRegion = uiState != MarkerUIStates.Collapsed ? 
                markerRegion : 
                new Rect(markerRegion.position,new Vector2(markerRegion.size.x,markerRegion.size.y/2f));
            
            var texture = markerEditorSettings.Normal;

            Color previousGuiColor = GUI.color;
            GUI.color = markerEditorSettings.DefaultColor;

            if (texture != null) {
                if (uiState == MarkerUIStates.Selected) {
                    GUI.color = markerEditorSettings.SelectedColor;
                } 
                GUI.DrawTexture(markerRegion, texture);
            }

            GUI.color = previousGuiColor; // Get back to previous GUI color

            var markers = marker.parent.GetMarkers();
            foreach (var otherMarker in markers) {
                if (marker == otherMarker) { continue;}

                if (Math.Abs(otherMarker.time - marker.time) < 0.01f) {
                    var plusTexture = markerEditorSettings.Plus;
                    
                    GUI.DrawTexture(new Rect(
                        markerRegion.position.x+2,
                        markerRegion.position.y+2,
                        10,10), plusTexture);
                }
            }
        }
    }
}