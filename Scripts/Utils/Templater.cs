using Godot;

public static class Templater
{
    public static void Template<T>(Node node, T def)
    {
        foreach (var child in node.GetChildren())
            Template(child, def);
        if (node is ITemplate<T> template)
            template.Template(def);
    }
}
