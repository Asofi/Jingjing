using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZ_Pooling;

public class StartFromBonus : MonoBehaviour {

    GUIManager guiManager;
    public float AnimationTime = 1;
    public ParticleSystem Particles;
    public TextMesh Text;

    private void Start()
    {
        guiManager = SuperManager.Instance.GUIManager;
        StartCoroutine(StarAnimation());
    }

    void OnSpawned()
    {
        StartCoroutine(StarAnimation());
        //Text.transform.localScale = Vector3.one * 0.016f;
        //Text.transform.localPosition = Vector2.zero;
    }

    IEnumerator StarAnimation()
    {
        if (guiManager == null)
            yield break;
        Transform _star = guiManager.Star;

        Text.transform.localScale = Vector3.one * 0.016f;

        Vector2 _startSize = Text.transform.localScale;
        Vector2 _endSize = _startSize * 1.8f;

        Vector2 _startPos = transform.position;
        Vector2 _endPos = (Vector2)transform.position + Vector2.up/1.5f + Vector2.right/3;

        Color _startAlpha = Text.color;
        Color _endAlpha = Text.color - new Color(0,0,0,1);
        //print(_startAlpha.a);
        //print(_endAlpha.a);
        float t = 0;
        Particles.Play();
        while (t <= 1) {
            Vector2 _newSize = Vector2.Lerp(_startSize, _endSize, t);
            Vector2 _newPos = Vector2.Lerp(_startPos, _endPos, t);

            Color _newColor = Color.Lerp(_startAlpha, _endAlpha, Mathf.Tan(1.5f * Mathf.Pow(t, 5)));
            Text.color = _newColor;
            Text.transform.localScale = _newSize;
            Text.transform.position = _newPos;
            
            t += Time.deltaTime * (1 / AnimationTime);
            yield return new WaitForEndOfFrame();
        }

        _startSize = Vector2.one * 0.5f;
        _endSize = Vector2.one * 1.2f * 0.5f;
        t = 0;
        while (t <= 1)
        {
            Vector2 _newSize = Vector2.Lerp(t< 0.5f?_startSize:_endSize, t<0.5f? _endSize:_startSize, t);
            _star.localScale = _newSize;
            t += Time.deltaTime * 4;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1);
        //print("Despawn Star");
        if (transform != null) {
            Text.color = _startAlpha;
            Text.transform.position = _startPos;
            EZ_PoolManager.Despawn(transform);
        }
    }
}
