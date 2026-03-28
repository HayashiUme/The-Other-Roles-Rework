using Hazel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using static TheOtherRoles.TheOtherRoles;
using HarmonyLib;
using Reactor.Utilities.Attributes;
using Reactor.Utilities.Extensions;
using Rewired.Utils.Platforms.Windows;
using TMPro;
using TheOtherRoles.CustomGameModes;

namespace TheOtherRoles.Objects
{
    public class Prop
    {
        [RegisterInIl2Cpp]
        public class Proptip : MonoBehaviour
        {
            public Proptip(IntPtr ptr) : base(ptr)
            {
            }

            public GameObject ProptipObj { get; set; }
            public TextMeshPro ProptipTMP { get; set; }
            public RectTransform ProptipTransform { get; set; }
            public MeshRenderer ProptipRenderer { get; set; }
            public bool Enabled { get; set; }
            public string ProptipText { get; set; }

            private void Start()
            {
                Enabled = true;

                ProptipObj = new GameObject().DontDestroy();
                ProptipObj.layer = 5;

                ProptipTMP = ProptipObj.AddComponent<TextMeshPro>();
                ProptipTMP.fontSize = 1.7f;
                ProptipTMP.alignment = TextAlignmentOptions.BottomLeft;
                ProptipTMP.overflowMode = TextOverflowModes.Overflow;
                ProptipTMP.maskable = false;

                ProptipRenderer = ProptipObj.GetComponent<MeshRenderer>();
                ProptipRenderer.sortingOrder = 1000;

                ProptipTransform = ProptipObj.GetComponent<RectTransform>();
                ProptipObj.SetActive(false);
            }

            public void OnDisable()
            {
                if (ProptipObj == null) return;
                ProptipObj.SetActive(false);
            }

            public void OnDestroy()
            {
                if (ProptipObj == null) return;
                ProptipObj.SetActive(false);
                ProptipObj.Destroy();
            }

            public void LateUpdate()
            {
                ProptipTransform.sizeDelta = ProptipTMP.GetPreferredValues(ProptipText);
                ProptipTMP.text = "<color=#EEFFB3FF>" + ProptipText + "</color>";

                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                ProptipObj.transform.position = new Vector3(mousePosition.x + (ProptipTMP.renderedWidth / 2) + 0.1f, mousePosition.y + (ProptipTMP.renderedHeight * 1.2f));
            }

            public void FixedUpdate()
            {
                ProptipObj.SetActive(false);
            }

            private void OnMouseOver()
            {
                if (!Enabled) return;
                ProptipObj.SetActive(true);
            }
        }
    }
}
