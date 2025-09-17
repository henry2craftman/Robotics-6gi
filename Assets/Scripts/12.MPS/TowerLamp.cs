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
            mat.DisableKeyword("_SURFACE_TYPE_TRANSPARENT"); // �Ϻ� URP ���� ���
            mat.SetFloat("_Blend", 0f);
            mat.SetFloat("_AlphaClip", 0f);
        }
        else
        {
            mat.SetFloat("_Surface", 1f);
            mat.SetOverrideTag("RenderType", "Transparent");
            mat.renderQueue = (int)RenderQueue.Transparent;

            mat.SetInt("_ZWrite", 0);
            mat.EnableKeyword("_ALPHABLEND_ON");            // �⺻ ���� ����
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
