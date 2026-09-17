// Assets/Plugins/ER-Inspector/Window/FieldConfigurationWindow.cs
using System;
using System.Collections.Generic;
using System.Linq;
using InspectorDesigner.Drawers;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace InspectorDesigner
{
    public class FieldConfigurationWindow : EditorWindow
    {
        private const string DragPayloadKey = "InspectorDesigner.DragPayload";
        private const string HelpBoxUssClassName = "unity-help-box";
        private const string WindowTitle = "ER Inspector";
        private const float ContentMinWidth = 460f;

        private const float RowHeight = 32f;
        private const float EditButtonWidth = 40f;
        private const float EditButtonMinWidth = 18f;
        private const float RemoveButtonSize = 20f;
        private const float ChildIndent = 18f;
        private const float ScriptFieldWidth = 260f;
        private const float ScriptFieldMinWidth = 120f;
        private const float ToolbarButtonPadding = 8f;
        private const int IdentifierFontSize = 9;

        private static readonly Color IdentifierLabelColor = new Color(0.55f, 0.55f, 0.55f, 1f);
        private static readonly Vector2 CursorPopupOffset = new Vector2(12f, 16f);
        private static readonly Vector2 WindowMinSize = new Vector2(360f, 260f);

        private static readonly HashSet<Type> HiddenFromAddMenu = new HashSet<Type>
        {
            typeof(VerticalGroupNode),
            typeof(HorizontalGroupNode)
        };

        private class DragPayload
        {
            public LayoutNode Node;
        }

        private class NodeConfigWindow : EditorWindow
        {
            private static readonly Vector2 DefaultSize = new Vector2(260f, 50f);
            private static readonly Vector2 InfoBoxSize = new Vector2(280f, 82f);

            private LayoutNode node;
            private Action onChanged;

            public static void Open(LayoutNode node, Vector2 screenPosition, Action onChanged)
            {
                var size = node is InfoBoxNode ? InfoBoxSize : DefaultSize;

                var window = CreateInstance<NodeConfigWindow>();
                window.node = node;
                window.onChanged = onChanged;
                window.titleContent = new GUIContent($"Edit {GetAttributeKindLabel(node)}");
                window.position = new Rect(screenPosition, size);
                window.minSize = size;
                window.maxSize = size;
                window.ShowUtility();
            }

            private void OnGUI()
            {
                EditorGUILayout.Space(4);
                EditorGUI.BeginChangeCheck();

                DrawFields();

                if (EditorGUI.EndChangeCheck()) onChanged?.Invoke();

                if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
                    Close();
            }

            private void DrawFields()
            {
                switch (node)
                {
                    case TitleNode title:
                        title.Text = EditorGUILayout.TextField("Title", title.Text);
                        break;

                    case InfoBoxNode box:
                        box.Message = EditorGUILayout.TextField("Message", box.Message);
                        box.Type = (InfoBoxNode.InfoBoxType)EditorGUILayout.EnumPopup("Type", box.Type);
                        break;

                    case FoldoutNode foldout:
                        foldout.Title = EditorGUILayout.TextField("Title", foldout.Title);
                        break;
                }
            }
        }

        private class TabConfigWindow : EditorWindow
        {
            private static readonly Vector2 DefaultSize = new Vector2(260f, 50f);

            private TabGroupNode.Tab tab;
            private Action onChanged;

            public static void Open(TabGroupNode.Tab tab, Vector2 screenPosition, Action onChanged)
            {
                var window = CreateInstance<TabConfigWindow>();
                window.tab = tab;
                window.onChanged = onChanged;
                window.titleContent = new GUIContent("Edit Tab");
                window.position = new Rect(screenPosition, DefaultSize);
                window.minSize = DefaultSize;
                window.maxSize = DefaultSize;
                window.ShowUtility();
            }

            private void OnGUI()
            {
                EditorGUILayout.Space(4);
                EditorGUI.BeginChangeCheck();

                tab.TabName = EditorGUILayout.TextField("Name", tab.TabName);

                if (EditorGUI.EndChangeCheck()) onChanged?.Invoke();

                if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
                    Close();
            }
        }

        [SerializeField] private MonoScript pinnedScript;

        private Type targetType;
        private InspectorLayoutConfig config;
        private ObjectField scriptField;
        private VisualElement nodesPanel;

        [MenuItem("Tools/ER Inspector")]
        public static void Open()
        {
            GetOrCreateWindowInstance();
        }

        [MenuItem("CONTEXT/MonoBehaviour/ER Inspector")]
        private static void OpenFromScriptContextMenu(MenuCommand command)
        {
            if (command.context is not MonoBehaviour behaviour) return;

            var window = GetOrCreateWindowInstance();
            window.SetPinnedScript(MonoScript.FromMonoBehaviour(behaviour));
        }

        private static FieldConfigurationWindow GetOrCreateWindowInstance()
        {
            var window = GetWindow<FieldConfigurationWindow>();
            window.titleContent = new GUIContent(WindowTitle);
            window.minSize = WindowMinSize;
            return window;
        }

        private void OnEnable()
        {
            BuildLayout();
            SetPinnedScript(pinnedScript);
        }

        private void SetPinnedScript(MonoScript script)
        {
            pinnedScript = script;
            scriptField?.SetValueWithoutNotify(script);
            SetTarget(script != null ? script.GetClass() : null);
        }

        private void SetTarget(Type type)
        {
            targetType = type;
            config = type != null ? ConfigAssetUtility.LoadOrCreate(type) : null;
            RebuildNodesPanel();
        }

        private void BuildLayout()
        {
            rootVisualElement.Clear();
            rootVisualElement.Add(BuildToolbar());
            rootVisualElement.Add(BuildNodesPanel());
        }

        private Toolbar BuildToolbar()
        {
            var toolbar = new Toolbar
            {
                style =
                {
                    flexShrink = 0,
                    alignItems = Align.Center
                }
            };

            scriptField = new ObjectField("Target Script")
            {
                objectType = typeof(MonoScript),
                value = pinnedScript,
                style =
                {
                    width = ScriptFieldWidth,
                    minWidth = ScriptFieldMinWidth,
                    flexGrow = 0,
                    flexShrink = 1
                }
            };

            scriptField.RegisterValueChangedCallback(evt => SetPinnedScript(evt.newValue as MonoScript));

            toolbar.Add(scriptField);
            toolbar.Add(CreateToolbarButton("Add Attribute", ShowAddAttributeMenu));
            toolbar.Add(CreateToolbarButton("Save", SaveConfig));

            return toolbar;
        }

        private static ToolbarButton CreateToolbarButton(string text, Action onClick)
        {
            var button = new ToolbarButton(onClick) { text = text };

            var style = button.style;
            style.flexGrow = 0;
            style.flexShrink = 0;
            style.width = StyleKeyword.Auto;
            style.minWidth = StyleKeyword.Auto;
            style.maxWidth = StyleKeyword.None;
            style.paddingLeft = ToolbarButtonPadding;
            style.paddingRight = ToolbarButtonPadding;
            style.whiteSpace = WhiteSpace.NoWrap;
            style.textOverflow = TextOverflow.Clip;
            style.unityTextAlign = TextAnchor.MiddleCenter;

            return button;
        }

        private VisualElement BuildNodesPanel()
        {
            nodesPanel = new ScrollView(ScrollViewMode.Vertical)
            {
                style =
                {
                    flexGrow = 1,
                    paddingLeft = 6,
                    paddingTop = 6,
                    paddingRight = 6,
                    paddingBottom = 6
                }
            };

            var content = nodesPanel.contentContainer;
            content.style.alignItems = Align.Stretch;
            content.style.flexGrow = 1;
            content.style.minWidth = 0;

            return nodesPanel;
        }

        private void ShowAddAttributeMenu()
        {
            var menu = new GenericMenu();

            if (config == null)
            {
                menu.AddDisabledItem(new GUIContent("Select the target script first"));
                menu.ShowAsContext();
                return;
            }

            foreach (var (nodeType, drawer) in LayoutNodeDrawerRegistry.AllAddable())
            {
                if (HiddenFromAddMenu.Contains(nodeType)) continue;

                menu.AddItem(new GUIContent(drawer.MenuLabel), false, () =>
                {
                    config.RootElements.Add((LayoutNode)Activator.CreateInstance(nodeType));
                    Refresh();
                });
            }

            menu.ShowAsContext();
        }

        private void EnsureDefaultFieldsPresent()
        {
            if (targetType == null || config == null) return;

            var inspectableFields = new HashSet<string>(FieldReflectionUtility.GetInspectableFieldNames(targetType));
            var placedFieldNames = LayoutTreeUtility.CollectFieldNames(config.RootElements);

            var staleNodes = new List<FieldNode>();
            CollectStaleFieldNodes(config.RootElements, inspectableFields, staleNodes);

            foreach (var node in staleNodes)
            {
                LayoutTreeUtility.Remove(config.RootElements, node);
            }

            foreach (var fieldName in inspectableFields)
            {
                if (placedFieldNames.Contains(fieldName)) continue;
                config.RootElements.Add(new FieldNode(fieldName));
            }
        }

        private void CollectStaleFieldNodes(List<LayoutNode> nodes, HashSet<string> validFields, List<FieldNode> result)
        {
            if (nodes == null) return;

            foreach (var node in nodes)
            {
                if (node is FieldNode fieldNode)
                {
                    if (!validFields.Contains(fieldNode.FieldName))
                    {
                        result.Add(fieldNode);
                    }
                }
                else if (node is FoldoutNode foldoutNode)
                {
                    CollectStaleFieldNodes(foldoutNode.Children, validFields, result);
                }
                else if (node is TabGroupNode tabGroupNode)
                {
                    foreach (var tab in tabGroupNode.Tabs)
                    {
                        CollectStaleFieldNodes(tab.Children, validFields, result);
                    }
                }
            }
        }

        private void RebuildNodesPanel()
        {
            if (nodesPanel == null) return;

            nodesPanel.Clear();

            if (targetType == null || config == null)
            {
                nodesPanel.Add(new Label("No target selected"));
                return;
            }

            EnsureDefaultFieldsPresent();

            nodesPanel.Add(new Label("Layout")
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold,
                    marginBottom = 4
                }
            });

            var layoutSection = new VisualElement
            {
                style =
                {
                    minHeight = 40,
                    alignSelf = Align.Stretch,
                    flexGrow = 0,
                    flexShrink = 0,
                    minWidth = 0
                }
            };

            RegisterListDropTarget(layoutSection, config.RootElements);

            foreach (var node in config.RootElements)
                layoutSection.Add(BuildNodeView(node, config.RootElements));

            nodesPanel.Add(layoutSection);
        }

        private VisualElement BuildNodeView(LayoutNode node, List<LayoutNode> containingList)
        {
            var wrapper = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Column,
                    alignSelf = Align.Stretch,
                    flexGrow = 0,
                    flexShrink = 0,
                    minWidth = 0
                }
            };

            wrapper.Add(BuildNodeRow(node, containingList));

            switch (node)
            {
                case FoldoutNode foldout:
                    wrapper.Add(BuildChildListContainer(foldout.Children));
                    break;

                case TabGroupNode tabGroup:
                    wrapper.Add(BuildTabGroupView(tabGroup));
                    break;
            }

            return wrapper;
        }

        private VisualElement BuildNodeRow(LayoutNode node, List<LayoutNode> containingList)
        {
            var row = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                    alignSelf = Align.Stretch,
                    flexGrow = 0,
                    flexShrink = 0,
                    height = RowHeight,
                    minWidth = 0,
                    marginBottom = 4,
                    paddingLeft = 6,
                    paddingRight = 4,
                    paddingTop = 3,
                    paddingBottom = 3,
                    overflow = Overflow.Hidden
                }
            };

            ApplyHelpBoxBackground(row);
            row.Add(BuildNodeContent(node));

            if (IsAttributeNode(node))
            {
                if (node is not TabGroupNode)
                    row.Add(CreateEditButton(evt => OpenNodeConfigPopup(node, evt)));

                row.Add(CreateRemoveButton(() =>
                {
                    LayoutTreeUtility.Remove(config.RootElements, node);
                    Refresh();
                }));
            }

            RegisterDragSource(row, new DragPayload { Node = node });
            RegisterRowDropTarget(row, node, containingList);

            return row;
        }

        private static bool IsAttributeNode(LayoutNode node)
        {
            return LayoutNodeDrawerRegistry.Get(node.GetType())?.MenuLabel != null;
        }

        private Vector2 GetScreenPositionFromClick(IPointerEvent evt)
        {
            return new Vector2(position.x + evt.position.x, position.y + evt.position.y) + CursorPopupOffset;
        }

        private void OpenNodeConfigPopup(LayoutNode node, ClickEvent evt)
        {
            NodeConfigWindow.Open(node, GetScreenPositionFromClick(evt), Refresh);
        }

        private void OpenTabConfigPopup(TabGroupNode.Tab tab, ClickEvent evt)
        {
            TabConfigWindow.Open(tab, GetScreenPositionFromClick(evt), Refresh);
        }

        private VisualElement BuildChildListContainer(List<LayoutNode> children)
        {
            var container = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Column,
                    alignSelf = Align.Stretch,
                    flexGrow = 0,
                    flexShrink = 0,
                    minWidth = 0,
                    marginLeft = ChildIndent,
                    marginBottom = 4,
                    paddingLeft = 2,
                    paddingRight = 2,
                    paddingTop = 4,
                    paddingBottom = 2,
                    minHeight = 20,
                    overflow = Overflow.Hidden
                }
            };

            ApplyHelpBoxBackground(container);
            RegisterListDropTarget(container, children);

            foreach (var child in children)
                container.Add(BuildNodeView(child, children));

            return container;
        }

        private VisualElement BuildTabGroupView(TabGroupNode tabGroup)
        {
            var container = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Column,
                    alignSelf = Align.Stretch,
                    flexGrow = 0,
                    flexShrink = 0,
                    minWidth = 0,
                    marginLeft = ChildIndent,
                    marginBottom = 4
                }
            };

            var tabsRow = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.FlexStart,
                    alignSelf = Align.Stretch,
                    flexGrow = 0,
                    flexShrink = 1,
                    minWidth = 0,
                    marginBottom = 2
                }
            };

            foreach (var tab in tabGroup.Tabs.ToList())
                tabsRow.Add(BuildTabColumn(tabGroup, tab));

            container.Add(tabsRow);

            container.Add(new Button(() =>
            {
                tabGroup.Tabs.Add(new TabGroupNode.Tab($"Tab {tabGroup.Tabs.Count + 1}"));
                Refresh();
            })
            {
                text = "+ Tab"
            });

            return container;
        }

        private VisualElement BuildTabColumn(TabGroupNode tabGroup, TabGroupNode.Tab tab)
        {
            var column = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Column,
                    flexGrow = 1,
                    flexShrink = 1,
                    flexBasis = 0,
                    alignSelf = Align.Stretch,
                    minWidth = 0,
                    marginRight = 4,
                    paddingLeft = 4,
                    paddingRight = 4,
                    paddingTop = 4,
                    paddingBottom = 4,
                    minHeight = RowHeight,
                    overflow = Overflow.Hidden
                }
            };

            ApplyHelpBoxBackground(column);
            column.Add(BuildTabHeader(tabGroup, tab));

            RegisterListDropTarget(column, tab.Children);

            foreach (var child in tab.Children)
                column.Add(BuildNodeView(child, tab.Children));

            return column;
        }

        private VisualElement BuildTabHeader(TabGroupNode tabGroup, TabGroupNode.Tab tab)
        {
            var header = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                    alignSelf = Align.Stretch,
                    flexGrow = 0,
                    flexShrink = 0,
                    minWidth = 0,
                    marginBottom = 3,
                    overflow = Overflow.Hidden
                }
            };

            header.Add(BuildAttributeContent("Tab", tab.TabName));
            header.Add(CreateEditButton(evt => OpenTabConfigPopup(tab, evt)));

            if (tabGroup.Tabs.Count > 1)
            {
                header.Add(CreateRemoveButton(() =>
                {
                    tabGroup.Tabs.Remove(tab);
                    Refresh();
                }));
            }

            return header;
        }

        private static Button CreateEditButton(EventCallback<ClickEvent> onClick)
        {
            var button = new Button { text = "Edit" };
            ApplyCompactButtonStyle(button, EditButtonWidth, EditButtonMinWidth, true);
            button.RegisterCallback(onClick);
            return button;
        }

        private static Button CreateRemoveButton(Action onClick)
        {
            var button = new Button(onClick) { text = "-" };
            ApplyCompactButtonStyle(button, RemoveButtonSize, RemoveButtonSize, false);
            return button;
        }

        private static void ApplyCompactButtonStyle(Button button, float width, float minWidth, bool shrinkable)
        {
            var style = button.style;
            style.width = width;
            style.minWidth = minWidth;
            style.maxWidth = width;
            style.flexGrow = 0;
            style.flexShrink = shrinkable ? 1 : 0;
            style.marginLeft = 2;
            style.marginRight = 0;
            style.paddingLeft = 1;
            style.paddingRight = 1;
            style.overflow = Overflow.Hidden;
            style.whiteSpace = WhiteSpace.NoWrap;
            style.textOverflow = TextOverflow.Clip;
            style.unityTextAlign = TextAnchor.MiddleCenter;
        }

        private static void ApplyHelpBoxBackground(VisualElement element)
        {
            element.AddToClassList(HelpBoxUssClassName);
        }

        private VisualElement BuildNodeContent(LayoutNode node)
        {
            if (node is FieldNode fieldNode)
                return BuildTruncatingLabel(ObjectNames.NicifyVariableName(fieldNode.FieldName));

            var kindLabel = GetAttributeKindLabel(node);
            if (kindLabel != null)
                return BuildAttributeContent(kindLabel, GetAttributeDisplayValue(node));

            return BuildTruncatingLabel(node.DisplayName);
        }

        private static Label BuildTruncatingLabel(string text)
        {
            return new Label(text)
            {
                style =
                {
                    flexGrow = 1,
                    flexShrink = 1,
                    minWidth = 0,
                    alignSelf = Align.Stretch,
                    overflow = Overflow.Hidden,
                    textOverflow = TextOverflow.Ellipsis,
                    whiteSpace = WhiteSpace.NoWrap,
                    unityTextAlign = TextAnchor.MiddleLeft
                }
            };
        }

        private VisualElement BuildAttributeContent(string kindLabel, string displayValue)
        {
            var column = new VisualElement
            {
                style =
                {
                    flexGrow = 1,
                    flexShrink = 1,
                    flexBasis = 0,
                    minWidth = 0,
                    overflow = Overflow.Hidden
                }
            };

            column.Add(new Label(kindLabel)
            {
                style =
                {
                    fontSize = IdentifierFontSize,
                    color = IdentifierLabelColor,
                    minWidth = 0,
                    overflow = Overflow.Hidden,
                    textOverflow = TextOverflow.Ellipsis,
                    whiteSpace = WhiteSpace.NoWrap,
                    unityFontStyleAndWeight = FontStyle.Normal,
                    unityTextAlign = TextAnchor.MiddleLeft
                }
            });

            if (!string.IsNullOrEmpty(displayValue))
            {
                column.Add(new Label(displayValue)
                {
                    style =
                    {
                        flexShrink = 1,
                        minWidth = 0,
                        overflow = Overflow.Hidden,
                        textOverflow = TextOverflow.Ellipsis,
                        whiteSpace = WhiteSpace.NoWrap,
                        unityTextAlign = TextAnchor.MiddleLeft
                    }
                });
            }

            return column;
        }

        private static string GetAttributeKindLabel(LayoutNode node) => node switch
        {
            TitleNode => "Title",
            InfoBoxNode => "Info Box",
            FoldoutNode => "Foldout",
            TabGroupNode => "Tab Group",
            _ => null
        };

        private static string GetAttributeDisplayValue(LayoutNode node) => node switch
        {
            TitleNode title => title.Text,
            InfoBoxNode box => box.Message,
            FoldoutNode foldout => foldout.Title,
            TabGroupNode => null,
            _ => null
        };

        private void RegisterDragSource(VisualElement source, DragPayload payload)
        {
            source.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button != 0) return;
                if (IsInteractiveElement(evt.target as VisualElement, source)) return;

                DragAndDrop.PrepareStartDrag();
                DragAndDrop.SetGenericData(DragPayloadKey, payload);
                DragAndDrop.objectReferences = Array.Empty<UnityEngine.Object>();
                DragAndDrop.StartDrag(payload.Node.DisplayName);
                evt.StopPropagation();
            });
        }

        private static bool IsInteractiveElement(VisualElement start, VisualElement stopAt)
        {
            var current = start;
            while (current != null && current != stopAt)
            {
                if (current is Button or TextField or EnumField or ObjectField or Toggle)
                    return true;
                current = current.parent;
            }
            return false;
        }

        private void RegisterListDropTarget(VisualElement target, List<LayoutNode> targetList)
        {
            RegisterDragUpdated(target);
            target.RegisterCallback<DragPerformEvent>(evt =>
            {
                if (DragAndDrop.GetGenericData(DragPayloadKey) is DragPayload payload)
                    PlaceNode(payload.Node, targetList, targetList.Count);

                DragAndDrop.AcceptDrag();
                evt.StopPropagation();
            });
        }

        private void RegisterRowDropTarget(VisualElement row, LayoutNode sibling, List<LayoutNode> siblingList)
        {
            RegisterDragUpdated(row);
            row.RegisterCallback<DragPerformEvent>(evt =>
            {
                if (DragAndDrop.GetGenericData(DragPayloadKey) is not DragPayload payload) return;

                var siblingIndex = siblingList.IndexOf(sibling);
                var dropBelow = evt.localMousePosition.y > row.layout.height / 2f;
                var insertIndex = Mathf.Max(0, siblingIndex + (dropBelow ? 1 : 0));

                PlaceNode(payload.Node, siblingList, insertIndex);
                DragAndDrop.AcceptDrag();
                evt.StopPropagation();
            });
        }

        private static void RegisterDragUpdated(VisualElement target)
        {
            target.RegisterCallback<DragUpdatedEvent>(evt =>
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                evt.StopPropagation();
            });
        }

        private void PlaceNode(LayoutNode node, List<LayoutNode> targetList, int insertIndex)
        {
            if (node == null) return;
            if (LayoutTreeUtility.IsListWithinSelfOrDescendant(node, targetList)) return;

            var previousIndexInTargetList = targetList.IndexOf(node);
            LayoutTreeUtility.Remove(config.RootElements, node);

            if (previousIndexInTargetList >= 0 && previousIndexInTargetList < insertIndex)
                insertIndex -= 1;

            insertIndex = Mathf.Clamp(insertIndex, 0, targetList.Count);
            targetList.Insert(insertIndex, node);
            Refresh();
        }

        private void Refresh()
        {
            RebuildNodesPanel();
        }

        private void SaveConfig()
        {
            if (config != null)
            {
                ConfigAssetUtility.Save(config);
                ShowNotification(new GUIContent("Layout saved successfully!"));
            }
        }
    }
}