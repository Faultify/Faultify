The configuration is stored in a secret


Run `dotnet user-secrets init` in `Faultinsertion.CompileTime.Cli` to initialize user-secrets

To edit settings you can right click `Faultinsertion.CompileTime.Cli` in VisualStudio and select `Manage User Secrets`

Example settings

```json
{
  "settings": {
    "targetFile": "Full PATH TO -> Faultinsertion.Tests\\bin\\Debug\\net5.0\\Faultinsertion.Target.dll"
  }
}
```
