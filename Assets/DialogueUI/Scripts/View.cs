using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueUI
{
    public class View : CustomUIComponent
    {
        public ViewSO viewData;

        public GameObject containerTop;
        public GameObject containerMiddle;
        public GameObject containerBotttom;

        private Image imageTop;
        private Image imageMiddle;
        private Image imageBotttom;

        private VerticalLayoutGroup verticalLayoutGroup;

        public override void Setup()
        {
            verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
            imageTop = containerTop.GetComponent<Image>();
            imageMiddle = containerMiddle.GetComponent<Image>();
            imageBotttom = containerBotttom.GetComponent<Image>();
        }

        public override void Configure()
        {
            verticalLayoutGroup.padding = viewData.padding;
            verticalLayoutGroup.spacing = viewData.spacing;

            imageTop.color = viewData.theme.primary_bg;
            imageMiddle.color = viewData.theme.secondary_bg;
            imageBotttom.color = viewData.theme.tertiary_bg;
        }
    }
}