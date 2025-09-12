using UnityEngine;
using UnityEngine.Rendering;

// ��ǥ : PLC�κ��� ��,Ȳ,��� ���� ��ȣ�� ������ �� object(��Ȳ��)�� ������ �����Ѵ�.
// �Ӽ� : �� object�� signal��, Renderer��
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
