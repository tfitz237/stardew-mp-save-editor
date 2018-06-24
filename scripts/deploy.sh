cd Stardew.MPSaveEditor
dotnet publish -c Release -r ${win_x86} -o $TRAVIS_BUILD_DIR/releases/Stardew.MPSaveEditor-${win_x86}
dotnet publish -c Release -r ${win_x64} -o $TRAVIS_BUILD_DIR/releases/Stardew.MPSaveEditor-${win_x64}
dotnet publish -c Release -r ${linux_x64} -o $TRAVIS_BUILD_DIR/releases/Stardew.MPSaveEditor-${linux_x64}
cd $TRAVIS_BUILD_DIR/releases
zip -r $TRAVIS_BUILD_DIR/releases/Stardew.MPSaveEditor-$TRAVIS_BUILD_ID-${win_x86}.zip ./Stardew.MPSaveEditor-${win_x86}
zip -r $TRAVIS_BUILD_DIR/releases/Stardew.MPSaveEditor-$TRAVIS_BUILD_ID-${win_x64}.zip ./Stardew.MPSaveEditor-${win_x64}
zip -r $TRAVIS_BUILD_DIR/releases/Stardew.MPSaveEditor-$TRAVIS_BUILD_ID-${linux_x64}.zip ./Stardew.MPSaveEditor-${linux_x64}
