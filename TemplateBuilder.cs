using System;
using System.Text;

internal sealed class TemplateBuilder
{
    public readonly StringBuilder BeforeResolveReferences = new StringBuilder();
    public readonly StringBuilder ModifyAppXPackage = new StringBuilder();

    public StringBuilder GetAdditionalTargets()
    {
        StringBuilder builder = new StringBuilder();
        bool flag = this.ModifyAppXPackage.Length > 0;
        if (flag)
        {
            builder.AppendLine("  <PropertyGroup>");
            builder.AppendLine("   <_GenerateAppxManifestDependsOn>");
            builder.AppendLine("    ModifyAppXPackage;");
            builder.AppendLine("    $(_GenerateAppxManifestDependsOn)");
            builder.AppendLine("   </_GenerateAppxManifestDependsOn>");
            builder.AppendLine("  </PropertyGroup>");
            builder.AppendLine();
        }
        if (this.BeforeResolveReferences.Length > 0)
        {
            builder.AppendLine("  <Target Name=\"BeforeResolveReferences\" Condition=\"'$(BuildingProject)' == 'true'\" > ");
            builder.Append(this.BeforeResolveReferences);
            builder.AppendLine("   </Target>");
        }
        if (flag)
        {
            builder.AppendLine("  <Target Name=\"ModifyAppXPackage\" Condition=\"'$(BuildingProject)' == 'true'\" > ");
            builder.Append(this.ModifyAppXPackage);
            builder.AppendLine("   </Target>");
        }
        builder.AppendLine("  <PropertyGroup>");
        builder.AppendLine("    <Win32Resource>Resource.res</Win32Resource>");
        builder.AppendLine("  </PropertyGroup>");
        builder.AppendLine();
        return builder;
    }
}

