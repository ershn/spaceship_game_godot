using Godot;

[GlobalClass]
public partial class WorkProgressBar : TextureProgressBar
{
    public void SetProgress(float ratio)
    {
        Visible = true;
        SetValueNoSignal(ratio);

        if (Mathf.IsEqualApprox(ratio, 1f))
            Reset();
    }

    public void Reset()
    {
        Visible = false;
        SetValueNoSignal(0d);
    }
}