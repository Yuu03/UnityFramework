﻿﻿﻿using UnityEditor;
using UnityEngine;

namespace Modules.Devkit.EditorStyleViewer
{
    public sealed class EditorStyleViewerWindow : EditorWindow
    {
        private static readonly string[] mList =
        {
            "AboutWIndowLicenseLabel"                       ,
            "AC LeftArrow"                                  ,
            "AC RightArrow"                                 ,
            "AnimationCurveEditorBackground"                ,
            "AnimationEventBackground"                      ,
            "AnimationEventTooltip"                         ,
            "AnimationEventTooltipArrow"                    ,
            "AnimationKeyframeBackground"                   ,
            "AnimationRowEven"                              ,
            "AnimationRowOdd"                               ,
            "AnimationSelectionTextField"                   ,
            "AnimationTimelineTick"                         ,
            "AnimPropDropdown"                              ,
            "AppToolbar"                                    ,
            "AS TextArea"                                   ,
            "BoldLabel"                                     ,
            "BoldToggle"                                    ,
            "ButtonLeft"                                    ,
            "ButtonMid"                                     ,
            "ButtonRight"                                   ,
            "CN Box"                                        ,
            "CN CountBadge"                                 ,
            "CN EntryBackEven"                              ,
            "CN EntryBackOdd"                               ,
            "CN EntryError"                                 ,
            "CN EntryInfo"                                  ,
            "CN EntryWarn"                                  ,
            "CN Message"                                    ,
            "CN StatusError"                                ,
            "CN StatusInfo"                                 ,
            "CN StatusWarn"                                 ,
            "ColorField"                                    ,
            "ColorPicker2DThumb"                            ,
            "ColorPickerBackground"                         ,
            "ColorPickerBox"                                ,
            "ColorPickerHorizThumb"                         ,
            "ColorPickerVertThumb"                          ,
            "Command"                                       ,
            "CommandLeft"                                   ,
            "CommandMid"                                    ,
            "CommandRight"                                  ,
            "ControlLabel"                                  ,
            "CurveEditorLabelTickmarks"                     ,
            "debug_layout_box"                              ,
            "dockarea"                                      ,
            "dockareaOverlay"                               ,
            "dockareaStandalone"                            ,
            "dragtab"                                       ,
            "dragtabbright"                                 ,
            "dragtabdropwindow"                             ,
            "DropDown"                                      ,
            "DropDownButton"                                ,
            "ErrorLabel"                                    ,
            "ExposablePopupItem"                            ,
            "ExposablePopupMenu"                            ,
            "EyeDropperHorizontalLine"                      ,
            "EyeDropperPickedPixel"                         ,
            "EyeDropperVerticalLine"                        ,
            "flow background"                               ,
            "flow navbar back"                              ,
            "flow navbar button"                            ,
            "flow navbar separator"                         ,
            "flow node 0"                                   ,
            "flow node 0 on"                                ,
            "flow node 1"                                   ,
            "flow node 1 on"                                ,
            "flow node 2"                                   ,
            "flow node 2 on"                                ,
            "flow node 3"                                   ,
            "flow node 3 on"                                ,
            "flow node 4"                                   ,
            "flow node 4 on"                                ,
            "flow node 5"                                   ,
            "flow node 5 on"                                ,
            "flow node 6"                                   ,
            "flow node 6 on"                                ,
            "flow node hex 0"                               ,
            "flow node hex 0 on"                            ,
            "flow node hex 1"                               ,
            "flow node hex 1 on"                            ,
            "flow node hex 2"                               ,
            "flow node hex 2 on"                            ,
            "flow node hex 3"                               ,
            "flow node hex 3 on"                            ,
            "flow node hex 4"                               ,
            "flow node hex 4 on"                            ,
            "flow node hex 5"                               ,
            "flow node hex 5 on"                            ,
            "flow node hex 6"                               ,
            "flow node hex 6 on"                            ,
            "flow node titlebar"                            ,
            "flow overlay area left"                        ,
            "flow overlay area right"                       ,
            "flow overlay box"                              ,
            "flow overlay foldout"                          ,
            "flow overlay header lower left"                ,
            "flow overlay header lower right"               ,
            "flow overlay header upper left"                ,
            "flow overlay header upper right"               ,
            "flow shader in 0"                              ,
            "flow shader in 1"                              ,
            "flow shader in 2"                              ,
            "flow shader in 3"                              ,
            "flow shader in 4"                              ,
            "flow shader in 5"                              ,
            "flow shader node 0"                            ,
            "flow shader node 0 on"                         ,
            "flow shader out 0"                             ,
            "flow shader out 1"                             ,
            "flow shader out 2"                             ,
            "flow shader out 3"                             ,
            "flow shader out 4"                             ,
            "flow shader out 5"                             ,
            "flow target in"                                ,
            "flow triggerPin in"                            ,
            "flow triggerPin out"                           ,
            "flow var 0"                                    ,
            "flow var 0 on"                                 ,
            "flow var 1"                                    ,
            "flow var 1 on"                                 ,
            "flow var 2"                                    ,
            "flow var 2 on"                                 ,
            "flow var 3"                                    ,
            "flow var 3 on"                                 ,
            "flow var 4"                                    ,
            "flow var 4 on"                                 ,
            "flow var 5"                                    ,
            "flow var 5 on"                                 ,
            "flow var 6"                                    ,
            "flow var 6 on"                                 ,
            "flow varPin in"                                ,
            "flow varPin out"                               ,
            "flow varPin tooltip"                           ,
            "Foldout"                                       ,
            "FoldOutPreDrop"                                ,
            "GameViewBackground"                            ,
            "Grad Down Swatch"                              ,
            "Grad Down Swatch Overlay"                      ,
            "Grad Up Swatch"                                ,
            "Grad Up Swatch Overlay"                        ,
            "grey_border"                                   ,
            "GridList"                                      ,
            "GridListText"                                  ,
            "GridToggle"                                    ,
            "GroupBox"                                      ,
            "GUIEditor.BreadcrumbLeft"                      ,
            "GUIEditor.BreadcrumbMid"                       ,
            "GV Gizmo DropDown"                             ,
            "HeaderLabel"                                   ,
            "HelpBox"                                       ,
            "Hi Label"                                      ,
            "HorizontalMinMaxScrollbarThumb"                ,
            "hostview"                                      ,
            "IN BigTitle"                                   ,
            "IN BigTitle Inner"                             ,
            "IN ColorField"                                 ,
            "IN DropDown"                                   ,
            "IN Foldout"                                    ,
            "IN FoldoutStatic"                              ,
            "IN Label"                                      ,
            "IN LockButton"                                 ,
            "IN ObjectField"                                ,
            "IN Popup"                                      ,
            "IN SelectedLine"                               ,
            "IN TextField"                                  ,
            "IN ThumbnailSelection"                         ,
            "IN ThumbnailShadow"                            ,
            "IN Title"                                      ,
            "IN TitleText"                                  ,
            "IN Toggle"                                     ,
            "InnerShadowBg"                                 ,
            "InvisibleButton"                               ,
            "LargeButton"                                   ,
            "LargeButtonLeft"                               ,
            "LargeButtonMid"                                ,
            "LargeButtonRight"                              ,
            "LargeDropDown"                                 ,
            "LargeLabel"                                    ,
            "LargePopup"                                    ,
            "LargeTextField"                                ,
            "LightmapEditorSelectedHighlight"               ,
            "ListToggle"                                    ,
            "LockedHeaderBackground"                        ,
            "LockedHeaderButton"                            ,
            "LockedHeaderLabel"                             ,
            "LODBlackBox"                                   ,
            "LODCameraLine"                                 ,
            "LODLevelNotifyText"                            ,
            "LODRendererAddButton"                          ,
            "LODRendererButton"                             ,
            "LODRendererRemove"                             ,
            "LODRenderersText"                              ,
            "LODSceneText"                                  ,
            "LODSliderBG"                                   ,
            "LODSliderRange"                                ,
            "LODSliderRangeSelected"                        ,
            "LODSliderText"                                 ,
            "LODSliderTextSelected"                         ,
            "MeBlendBackground"                             ,
            "MeBlendPosition"                               ,
            "MeBlendTriangleLeft"                           ,
            "MeBlendTriangleRight"                          ,
            "MeLivePlayBackground"                          ,
            "MeLivePlayBar"                                 ,
            "MeTimeLabel"                                   ,
            "MeTransBGOver"                                 ,
            "MeTransitionBack"                              ,
            "MeTransitionBlock"                             ,
            "MeTransitionHandleLeft"                        ,
            "MeTransitionHandleLeftPrev"                    ,
            "MeTransitionHandleRight"                       ,
            "MeTransitionHead"                              ,
            "MeTransitionSelect"                            ,
            "MeTransitionSelectHead"                        ,
            "MeTransOff2On"                                 ,
            "MeTransOffLeft"                                ,
            "MeTransOffRight"                               ,
            "MeTransOn2Off"                                 ,
            "MeTransOnLeft"                                 ,
            "MeTransOnRight"                                ,
            "MeTransPlayhead"                               ,
            "MiniBoldLabel"                                 ,
            "minibutton"                                    ,
            "minibuttonleft"                                ,
            "minibuttonmid"                                 ,
            "minibuttonright"                               ,
            "MiniLabel"                                     ,
            "MiniLabelRight"                                ,
            "MiniMinMaxSliderHorizontal"                    ,
            "MiniMinMaxSliderVertical"                      ,
            "MiniPopup"                                     ,
            "MiniPullDown"                                  ,
            "MiniPullDownLeft"                              ,
            "MiniTextField"                                 ,
            "MiniToolbarButton"                             ,
            "MiniToolbarButtonLeft"                         ,
            "MiniToolbarPopup"                              ,
            "MinMaxHorizontalSliderThumb"                   ,
            "NotificationBackground"                        ,
            "NotificationText"                              ,
            "ObjectField"                                   ,
            "ObjectFieldThumb"                              ,
            "ObjectFieldThumbOverlay"                       ,
            "ObjectFieldThumbOverlay2"                      ,
            "ObjectPickerBackground"                        ,
            "ObjectPickerGroupHeader"                       ,
            "ObjectPickerLargeStatus"                       ,
            "ObjectPickerPreviewBackground"                 ,
            "ObjectPickerResultsEven"                       ,
            "ObjectPickerResultsGrid"                       ,
            "ObjectPickerResultsGridLabel"                  ,
            "ObjectPickerResultsOdd"                        ,
            "ObjectPickerSmallStatus"                       ,
            "ObjectPickerTab"                               ,
            "ObjectPickerToolbar"                           ,
            "OL box"                                        ,
            "OL box NoExpand"                               ,
            "OL Elem"                                       ,
            "OL EntryBackEven"                              ,
            "OL EntryBackOdd"                               ,
            "OL header"                                     ,
            "OL Label"                                      ,
            "OL Minus"                                      ,
            "OL Plus"                                       ,
            "OL TextField"                                  ,
            "OL Title"                                      ,
            "OL Title TextRight"                            ,
            "OL Titleleft"                                  ,
            "OL Titlemid"                                   ,
            "OL Titleright"                                 ,
            "OL Toggle"                                     ,
            "OL ToggleWhite"                                ,
            "PaneOptions"                                   ,
            "PlayerSettingsLevel"                           ,
            "PlayerSettingsPlatform"                        ,
            "Popup"                                         ,
            "PopupBackground"                               ,
            "PopupCurveDropdown"                            ,
            "PopupCurveEditorBackground"                    ,
            "PopupCurveEditorSwatch"                        ,
            "PopupCurveSwatchBackground"                    ,
            "PR DigDownArrow"                               ,
            "PR Insertion"                                  ,
            "PR Label"                                      ,
            "PR Ping"                                       ,
            "PR TextField"                                  ,
            "PreBackground"                                 ,
            "PreButton"                                     ,
            "PreferencesKeysElement"                        ,
            "PreferencesSection"                            ,
            "PreferencesSectionBox"                         ,
            "PreHorizontalScrollbar"                        ,
            "PreHorizontalScrollbarThumb"                   ,
            "PreLabel"                                      ,
            "PreOverlayLabel"                               ,
            "PreSlider"                                     ,
            "PreSliderThumb"                                ,
            "PreToolbar"                                    ,
            "PreToolbar2"                                   ,
            "PreVerticalScrollbar"                          ,
            "PreVerticalScrollbarThumb"                     ,
            "ProfilerBadge"                                 ,
            "ProfilerLeftPane"                              ,
            "ProfilerLeftPaneOverlay"                       ,
            "ProfilerPaneLeftBackground"                    ,
            "ProfilerPaneSubLabel"                          ,
            "ProfilerRightPane"                             ,
            "ProfilerScrollviewBackground"                  ,
            "ProfilerSelectedLabel"                         ,
            "ProgressBarBack"                               ,
            "ProgressBarBar"                                ,
            "ProgressBarText"                               ,
            "ProjectBrowserBottomBarBg"                     ,
            "ProjectBrowserGridLabel"                       ,
            "ProjectBrowserHeaderBgMiddle"                  ,
            "ProjectBrowserHeaderBgTop"                     ,
            "ProjectBrowserIconAreaBg"                      ,
            "ProjectBrowserIconDropShadow"                  ,
            "ProjectBrowserPreviewBg"                       ,
            "ProjectBrowserSubAssetBg"                      ,
            "ProjectBrowserSubAssetBgCloseEnded"            ,
            "ProjectBrowserSubAssetBgDivider"               ,
            "ProjectBrowserSubAssetBgMiddle"                ,
            "ProjectBrowserSubAssetBgOpenEnded"             ,
            "ProjectBrowserSubAssetExpandBtn"               ,
            "ProjectBrowserTopBarBg"                        ,
            "QualitySettingsDefault"                        ,
            "Radio"                                         ,
            "RightLabel"                                    ,
            "RL Background"                                 ,
            "RL DragHandle"                                 ,
            "RL Element"                                    ,
            "RL Footer"                                     ,
            "RL FooterButton"                               ,
            "RL Header"                                     ,
            "SC ViewAxisLabel"                              ,
            "SC ViewLabel"                                  ,
            "SceneViewOverlayTransparentBackground"         ,
            "ScriptText"                                    ,
            "SearchCancelButton"                            ,
            "SearchCancelButtonEmpty"                       ,
            "SearchModeFilter"                              ,
            "SearchTextField"                               ,
            "SelectionRect"                                 ,
            "ServerChangeCount"                             ,
            "ServerUpdateChangeset"                         ,
            "ServerUpdateChangesetOn"                       ,
            "ServerUpdateInfo"                              ,
            "ServerUpdateLog"                               ,
            "ShurikenCheckMark"                             ,
            "ShurikenEffectBg"                              ,
            "ShurikenEmitterTitle"                          ,
            "ShurikenLabel"                                 ,
            "ShurikenLine"                                  ,
            "ShurikenMinus"                                 ,
            "ShurikenModuleBg"                              ,
            "ShurikenModuleTitle"                           ,
            "ShurikenObjectField"                           ,
            "ShurikenPlus"                                  ,
            "ShurikenPopUp"                                 ,
            "ShurikenToggle"                                ,
            "ShurikenValue"                                 ,
            "SimplePopup"                                   ,
            "SliderMixed"                                   ,
            "StaticDropdown"                                ,
            "sv_iconselector_back"                          ,
            "sv_iconselector_button"                        ,
            "sv_iconselector_labelselection"                ,
            "sv_iconselector_selection"                     ,
            "sv_iconselector_sep"                           ,
            "sv_label_0"                                    ,
            "sv_label_1"                                    ,
            "sv_label_2"                                    ,
            "sv_label_3"                                    ,
            "sv_label_4"                                    ,
            "sv_label_5"                                    ,
            "sv_label_6"                                    ,
            "sv_label_7"                                    ,
            "TabWindowBackground"                           ,
            "Tag MenuItem"                                  ,
            "Tag TextField"                                 ,
            "Tag TextField Button"                          ,
            "Tag TextField Empty"                           ,
            "TE NodeBackground"                             ,
            "TE NodeBox"                                    ,
            "TE NodeBoxSelected"                            ,
            "TE NodeLabelBot"                               ,
            "TE NodeLabelTop"                               ,
            "TE PinLabel"                                   ,
            "TE Toolbar"                                    ,
            "TE toolbarbutton"                              ,
            "TE ToolbarDropDown"                            ,
            "TimeScrubber"                                  ,
            "TimeScrubberButton"                            ,
            "TL BaseStateLogicBarOverlay"                   ,
            "TL EndPoint"                                   ,
            "TL InPoint"                                    ,
            "TL ItemTitle"                                  ,
            "TL LeftColumn"                                 ,
            "TL LeftItem"                                   ,
            "TL LogicBar 0"                                 ,
            "TL LogicBar 1"                                 ,
            "TL LogicBar parentgrey"                        ,
            "TL LoopSection"                                ,
            "TL OutPoint"                                   ,
            "TL Playhead"                                   ,
            "TL Range Overlay"                              ,
            "TL RightLine"                                  ,
            "TL Selection H1"                               ,
            "TL Selection H2"                               ,
            "TL SelectionBarCloseButton"                    ,
            "TL SelectionBarPreview"                        ,
            "TL SelectionBarText"                           ,
            "TL SelectionButton"                            ,
            "TL SelectionButton PreDropGlow"                ,
            "TL SelectionButtonName"                        ,
            "TL SelectionButtonNew"                         ,
            "TL tab left"                                   ,
            "TL tab mid"                                    ,
            "TL tab plus left"                              ,
            "TL tab plus right"                             ,
            "TL tab right"                                  ,
            "ToggleMixed"                                   ,
            "Toolbar"                                       ,
            "toolbarbutton"                                 ,
            "ToolbarDropDown"                               ,
            "ToolbarPopup"                                  ,
            "ToolbarSeachCancelButton"                      ,
            "ToolbarSeachCancelButtonEmpty"                 ,
            "ToolbarSeachTextField"                         ,
            "ToolbarSeachTextFieldPopup"                    ,
            "ToolbarSearchField"                            ,
            "ToolbarTextField"                              ,
            "Tooltip"                                       ,
            "U2D.createRect"                                ,
            "U2D.dragDot"                                   ,
            "U2D.dragDotDimmed"                             ,
            "VCS_StickyNote"                                ,
            "VCS_StickyNoteArrow"                           ,
            "VCS_StickyNoteLabel"                           ,
            "VCS_StickyNoteP4"                              ,
            "VerticalMinMaxScrollbarThumb"                  ,
            "VisibilityToggle"                              ,
            "WhiteBoldLabel"                                ,
            "WhiteLabel"                                    ,
            "WhiteLargeLabel"                               ,
            "WhiteMiniLabel"                                ,
            "WinBtnCloseActiveMac"                          ,
            "WinBtnCloseMac"                                ,
            "WinBtnCloseWin"                                ,
            "WinBtnInactiveMac"                             ,
            "WinBtnMaxActiveMac"                            ,
            "WinBtnMaxMac"                                  ,
            "WinBtnMaxWin"                                  ,
            "WinBtnMinActiveMac"                            ,
            "WinBtnMinMac"                                  ,
            "WinBtnMinWin"                                  ,
            "WindowBackground"                              ,
            "WindowBottomResize"                            ,
            "WindowResizeMac"                               ,
            "Wizard Box"                                    ,
            "Wizard Error"                                  ,
            "WordWrapLabel"                                 ,
            "WordWrappedLabel"                              ,
            "WordWrappedMiniLabel"                          ,
            "WrappedLabel"                                  ,
        };

        private Vector2 mScrollPos;

        public static void Open()
        {
            GetWindow<EditorStyleViewerWindow>(true);
        }

        private void OnGUI()
        {
            mScrollPos = EditorGUILayout.BeginScrollView(mScrollPos);
            foreach (var n in mList)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Height(48));
                EditorGUILayout.SelectableLabel(n);
                EditorGUILayout.Toggle(false, n);
                EditorGUILayout.EndHorizontal();
                GUILayout.Box(
                    string.Empty,
                    GUILayout.Width(position.width - 24),
                    GUILayout.Height(1)
                );
            }
            EditorGUILayout.EndScrollView();
        }
    }
}