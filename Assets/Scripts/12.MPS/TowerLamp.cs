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
        
        // URP Lit: _Surface 0=Opaque, 1=Transparent
        mat.SetInt("_Surface", makeOpaque ? 0 : 1);

        CoreUtils.SetKeyword(mat, "_SURFACE_TYPE_TRANSPARENT", !makeOpaque);
        CoreUtils.SetKeyword(mat, "_SURFACE_TYPE_OPAQUE", makeOpaque);

        if (makeOpaque)
        {
            mat.SetOverrideTag("RenderType", "Opaque");
            mat.renderQueue = (int)RenderQueue.Geometry;
        }
        else
        {
            mat.SetOverrideTag("RenderType", "Transparent");
            mat.renderQueue = (int)RenderQueue.Transparent;
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
