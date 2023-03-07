using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public static class Action2D
    {
        public static IEnumerator MoveTo(Transform target, Vector3 to, float duration, bool isSelfRemove = false)
        {
            Vector3 startPos = target.transform.position;

            float elapsedTime = 0.0f;
            while(elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                target.transform.position = Vector2.Lerp(startPos, to, elapsedTime / duration);

                yield return null;
            }

            target.transform.position = to;

            if (isSelfRemove)
                Object.Destroy(target.gameObject, 0.1f);

            yield break;
        }
    }
}
