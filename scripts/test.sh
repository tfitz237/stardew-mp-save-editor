cd Stardew.MPSaveEditor.Tests
[ -e ../Stardew.MPSaveEditor/Stardew.MPSaveEditor.csproj ] && echo "Project Found." || echo "Project not found."
[ -e /home/travis/build/tfitz237/stardew-mp-save-editor/Stardew.MPSaveEditor/Stardew.MPSaveEditor.csproj ] && echo "Project Found." || echo "Project not found."
[ -e home/travis/build/tfitz237/stardew-mp-save-editor/Stardew.MPSaveEditor/Stardew.MPSaveEditor.csproj ] && echo "Project Found." || echo "Project not found."
dotnet test