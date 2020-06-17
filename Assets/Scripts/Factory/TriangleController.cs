using System;
using UnityEngine;

namespace Factory
{
    public class TriangleController : MonoBehaviour
    {
        public enum TriangleColor
        {
            None,
            Yellow,
            Orange,
            Red
        }

        [SerializeField]
        private GameObject _yellow;

        [SerializeField]
        private GameObject _orange;

        [SerializeField]
        private GameObject _red;

        public void SetColor(TriangleColor triangleColor)
        {
            switch (triangleColor)
            {
                case TriangleColor.None:
                    _yellow.SetActive(false);
                    _orange.SetActive(false);
                    _red.SetActive(false);
                    break;
                case TriangleColor.Yellow:
                    _yellow.SetActive(true);
                    _orange.SetActive(false);
                    _red.SetActive(false);
                    break;
                case TriangleColor.Orange:
                    _yellow.SetActive(false);
                    _orange.SetActive(true);
                    _red.SetActive(false);
                    break;
                case TriangleColor.Red:
                    _yellow.SetActive(false);
                    _orange.SetActive(false);
                    _red.SetActive(true);
                    break;
            }
        }
    }
}