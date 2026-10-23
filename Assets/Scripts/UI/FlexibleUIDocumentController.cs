/*
 * (C) 2023 Radrat Softworks
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace Nofun.UI
{
    public class FlexibleUIDocumentController : MonoBehaviour
    {
        [Inject] protected ScreenManager screenManager;
        [Inject] protected DocumentStackManager documentStackManager;

        protected UIDocument document;

        private Rect appliedSafeArea;
        private IPanel appliedSafeAreaPanel;

        public UIDocument Document => document;

        private void UpdatePanelSettings()
        {
            document.panelSettings = screenManager.CurrentPanelSettings;
        }

        private void OnOrientationChanged(Settings.ScreenOrientation newOrientation)
        {
            UpdatePanelSettings();
        }

        public virtual void Awake()
        {
            document = GetComponent<UIDocument>();
        }

        public virtual void Start()
        {
            UpdatePanelSettings();
            screenManager.ScreenOrientationChanged += OnOrientationChanged;
        }

        private void LateUpdate()
        {
            if (Application.isMobilePlatform)
            {
                ApplySafeArea();
            }
        }

        /// <summary>
        /// Keep the document content out of the notch and rounded corners on phones.
        /// </summary>
        private void ApplySafeArea()
        {
            VisualElement root = document.rootVisualElement;
            Rect safeArea = Screen.safeArea;

            if ((root?.panel == null) || ((safeArea == appliedSafeArea) && (root.panel == appliedSafeAreaPanel)))
            {
                return;
            }

            // Screen space has its origin at the bottom-left, panel space at the top-left
            Vector2 topLeftInset = RuntimePanelUtils.ScreenToPanel(root.panel,
                new Vector2(safeArea.xMin, Screen.height - safeArea.yMax));
            Vector2 bottomRightInset = RuntimePanelUtils.ScreenToPanel(root.panel,
                new Vector2(Screen.width - safeArea.xMax, safeArea.yMin));

            root.style.paddingLeft = topLeftInset.x;
            root.style.paddingTop = topLeftInset.y;
            root.style.paddingRight = bottomRightInset.x;
            root.style.paddingBottom = bottomRightInset.y;

            appliedSafeArea = safeArea;
            appliedSafeAreaPanel = root.panel;
        }
    }
}
