#if TOOLS
using Godot;

namespace AmountInspector;

public partial class Property : EditorProperty
{
    const string PropertyScenePath = "res://addons/AmountInspector/property.tscn";

    static readonly StringName s_amountTypeMetaName = new("_amountType");

    readonly Hint.Type _hintType;
    bool AnyAmountType => _hintType == Hint.Type.Any;
    readonly string _hintParam;

    AmountType? _amountType;
    AmountType EffectiveAmountType => _amountType ?? AmountType.Count;
    ulong AmountValue => (ulong)GetEditedObject().Get(GetEditedProperty());
    string AmountValueText => AmountString.Format(EffectiveAmountType, AmountValue);

    readonly Control _property;
    readonly LineEdit _valueInput;
    readonly TextureRect _valueErrorIcon;
    readonly TextureRect _typeWarningIcon;
    readonly TextureRect _anyTypeWarningIcon;
    bool _validInputValue;

    Property() { }

    public Property(Hint.Type hintType, string hintParam, EditorInterface editorInterface)
    {
        var theme = editorInterface.GetBaseControl().Theme;

        _hintType = hintType;
        _hintParam = hintParam;

        _property = GD.Load<PackedScene>(PropertyScenePath).Instantiate<Control>();

        _valueInput = _property.GetNode<LineEdit>("ValueInput");
        _valueInput.TextChanged += InputValueChanged;
        _valueInput.TextSubmitted += InputValueSubmitted;

        _valueErrorIcon = _property.GetNode<TextureRect>("ValueErrorIcon");
        _valueErrorIcon.Texture = theme.GetIcon("StatusError", "EditorIcons");

        _typeWarningIcon = _property.GetNode<TextureRect>("TypeWarningIcon");
        _typeWarningIcon.Texture = theme.GetIcon("NodeWarning", "EditorIcons");

        _anyTypeWarningIcon = _property.GetNode<TextureRect>("AnyTypeWarningIcon");
        _anyTypeWarningIcon.Texture = theme.GetIcon("StatusWarning", "EditorIcons");
        _anyTypeWarningIcon.Visible = AnyAmountType;

        AddChild(_property);
        AddFocusable(_property);
    }

    void InputValueChanged(string text)
    {
        var validAmountType = UpdateAmountType();
        _validInputValue = UpdateAmountValue(text);

        _valueErrorIcon.Visible = !_validInputValue;
        _typeWarningIcon.Visible = !validAmountType;
    }

    void InputValueSubmitted(string _text)
    {
        if (_validInputValue)
            UpdateProperty();
    }

    public override void _UpdateProperty()
    {
        var validAmountType = UpdateAmountType();

        _valueInput.Text = AmountValueText;
        _valueErrorIcon.Visible = false;
        _typeWarningIcon.Visible = !validAmountType;
    }

    bool UpdateAmountType()
    {
        if (AnyAmountType)
        {
            _amountType = GetMetaAmountType();
            return true;
        }
        else
        {
            _amountType = GetAmountType();
            return _amountType is not null;
        }
    }

    bool UpdateAmountValue(string text)
    {
        if (AnyAmountType)
        {
            if (AmountString.TryParse(text, out var type, out var value))
            {
                _amountType = type;
                SetMetaAmountType(type);
                EmitChanged(GetEditedProperty(), value, changing: true);
                return true;
            }
        }
        else
        {
            if (AmountString.TryParse(EffectiveAmountType, text, out var value))
            {
                EmitChanged(GetEditedProperty(), value, changing: true);
                return true;
            }
        }
        return false;
    }

    AmountType? GetAmountType() =>
        _hintType switch
        {
            Hint.Type.Mass => AmountType.Mass,
            Hint.Type.Count => AmountType.Count,
            Hint.Type.ModeOf => GetAmountModeOf(_hintParam.Split('/'))?.AmountType,
            _ => throw new System.NotImplementedException()
        };

    AmountMode GetAmountModeOf(string[] propertyPath)
    {
        var obj = GetEditedObject();
        foreach (var property in propertyPath)
        {
            obj = (GodotObject)obj.Get(property);
            if (obj is null)
                return null;
        }
        return (obj as IAmountModeGet)?.AmountMode;
    }

    AmountType? GetMetaAmountType()
    {
        var obj = GetEditedObject();
        if (obj.HasMeta(s_amountTypeMetaName))
            return (AmountType?)(int)obj.GetMeta(s_amountTypeMetaName);
        else
            return null;
    }

    void SetMetaAmountType(AmountType amountType)
    {
        GetEditedObject().SetMeta(s_amountTypeMetaName, (int)amountType);
    }
}
#endif
