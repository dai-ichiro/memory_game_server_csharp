## Project Overview

This is a Windows Forms desktop application (.NET Framework 4.8) with a minimal structure. The main entry point is `Program.cs` which initializes and runs `Form1`, the primary UI form.

## Build and Run

```bash
# Debug build
powershell.exe -Command "& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' WinForm.csproj -p:Configuration=Debug"

# Release build
powershell.exe -Command "& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' WinForm.csproj -p:Configuration=Release"
```
