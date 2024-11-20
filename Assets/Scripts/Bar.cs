using UnityEngine;

namespace GlobalUtilities
{
    public static class Bar
    {
        public static string Create(float value)
        {
            value = Mathf.Clamp01(value);
            int totalSegments = 10;
            int filledSegments = Mathf.FloorToInt(value * totalSegments);
            int halfFilledSegments = Mathf.FloorToInt((value * totalSegments - filledSegments) * 2);

            string filledPart = new string('#', filledSegments);
            string halfFilledPart = new string('=', halfFilledSegments);
            string emptyPart = new string('.', totalSegments - filledSegments - halfFilledSegments);

            return $"[{filledPart}{halfFilledPart}{emptyPart}]";
        }
    }
}