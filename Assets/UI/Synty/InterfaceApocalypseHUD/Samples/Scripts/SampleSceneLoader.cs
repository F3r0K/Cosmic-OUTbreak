// Copyright (c) 2023 Synty Studios Limited. All rights reserved.
//
// Use of this software is subject to the terms and conditions of the Synty Studios End User Licence Agreement (EULA)
// available at: https://syntystore.com/pages/end-user-licence-agreement
//
// Sample scripts are included only as examples and are not intended as production-ready.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Synty.Interface.Samples
{
    /// <summary>
    ///     A simple scene loader that transitions between screens.
    /// </summary>
    public class SampleSceneLoader : MonoBehaviour
    {
        [Header("References")]
        public Animator animator;
        public CanvasGroup canvasGroup;
        public RectTransform titleScreen;
        public RectTransform contentsScreen;
        public TextMeshProUGUI titleText;
        public RectTransform contentParent;

        [Header("Parameters")]
        public bool showCursor;
        private List<RectTransform> contentList = new List<RectTransform>();
        private RectTransform currentContent;

        private void Awake()
        {
            contentList = contentParent.GetComponentsInChildren<RectTransform>(true)
                .Where(screen => screen.parent == contentParent)
                .ToList();
            contentList.Insert(0, null);

            titleScreen.gameObject.SetActive(true);
            contentsScreen.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (animator)
            {
                animator.gameObject.SetActive(true);
                animator.SetBool("Active", false);
            }

            if (showCursor)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void QuitApplication()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void ActivateContent(int index)
        {
            StartCoroutine(C_ActivateContent(index));
        }

        private IEnumerator C_ActivateContent(int index)
        {
            canvasGroup.interactable = false;

            if (animator)
            {
                animator.gameObject.SetActive(true);
                animator.SetBool("Active", true);

                yield return new WaitForSeconds(0.4f);

                animator.SetBool("Active", false);
            }

            foreach (RectTransform content in contentList)
            {
                if (content)
                {
                    content.gameObject.SetActive(false);
                }
            }

            currentContent = contentList[index];
            currentContent.gameObject.SetActive(true);
            titleText.text = currentContent.name;

            titleScreen.gameObject.SetActive(false);
            contentsScreen.gameObject.SetActive(true);

            canvasGroup.interactable = true;
        }

        public void ActivateNextContent()
        {
            int currentIndex = contentList.IndexOf(currentContent);
            int nextIndex = currentIndex + 1;
            if (nextIndex >= contentList.Count)
            {
                nextIndex = 1;
            }

            ActivateContent(nextIndex);
        }

        public void ActivatePreviousContent()
        {
            int currentIndex = contentList.IndexOf(currentContent);
            int previousIndex = currentIndex - 1;
            if (previousIndex < 1)
            {
                previousIndex = contentList.Count - 1;
            }

            ActivateContent(previousIndex);
        }

        public void ActivateTitleScreen()
        {
            StartCoroutine(C_ActivateTitleScreen());
        }

        private IEnumerator C_ActivateTitleScreen()
        {
            canvasGroup.interactable = false;

            if (animator)
            {
                animator.gameObject.SetActive(true);
                animator.SetBool("Active", true);

                yield return new WaitForSeconds(0.4f);

                animator.SetBool("Active", false);
            }

            titleScreen.gameObject.SetActive(true);
            contentsScreen.gameObject.SetActive(false);

            canvasGroup.interactable = true;
        }
    }
}
