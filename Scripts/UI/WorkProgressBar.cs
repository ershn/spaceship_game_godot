using Godot;

[GlobalClass]
public partial class WorkProgressBar : TextureProgressBar
{
    public void SetProgress(float ratio)
    {
        Show();
        SetValueNoSignal(ratio);

        if (Mathf.IsEqualApprox(ratio, 1f))
            Reset();
    }

    public void Reset()
    {
        Hide();
        SetValueNoSignal(0d);
    }
}
