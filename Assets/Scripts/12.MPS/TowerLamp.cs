using UnityEngine;
using UnityEngine.Rendering;

// 목표 : PLC로부터 적,황,녹색 램프 신호를 받으면 각 object(적황녹)의 색상을 변경한다.
// 속성 : 각 object의 signal들, Renderer들
public class TowerLamp : MonoBehaviour
{
    public bool isRedSignal = false;
    public bool isYelSignal = false;
    public bool isGrnSignal = false;

    [SerializeField] Renderer redLamp;
    [SerializeField] Renderer yellowLamp;
    [SerializeField] Renderer greenLamp;

    void SetOpaque(Renderer render, bool makeOpaque)
    {
        if (render == null) return;

        var mat = render.material;
        if (!mat) return;

        if (makeOpaque)
        {
            mat.SetFloat("_Surface", 0f);
            mat.SetOverrideTag("RenderType", "Opaque");
            mat.renderQueue = (int)RenderQueue.Geometry;

            mat.SetInt("_ZWrite", 1);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.DisableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.DisableKeyword("_SURFACE_TYPE_TRANSPARENT"); // 일부 URP 버전 대비
            mat.SetFloat("_Blend", 0f);
            mat.SetFloat("_AlphaClip", 0f);
        }
        else
        {
            mat.SetFloat("_Surface", 1f);
            mat.SetOverrideTag("RenderType", "Transparent");
            mat.renderQueue = (int)RenderQueue.Transparent;

            mat.SetInt("_ZWrite", 0);
            mat.EnableKeyword("_ALPHABLEND_ON");            // 기본 알파 블렌딩
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.SetFloat("_Blend", 0f);
        }
        
    }

    public void OnRedLampClkEvent()
    {
        isRedSignal = !isRedSignal;
        SetOpaque(redLamp, isRedSignal);
    }

    public void OnYellowLampClkEvent()
    {
        isYelSignal = !isYelSignal;
        SetOpaque(yellowLamp, isYelSignal);
    }

    public void OnGreenLampClkEvent()
    {
        isGrnSignal = !isGrnSignal;
        SetOpaque(greenLamp, isGrnSignal);
    }
}
