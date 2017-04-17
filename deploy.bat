
set H=R:\KSP_1.2.2_dev
echo %H%

copy /Y "Source\bin\Debug\CrewRandR.dll" "GameData\CrewRandR\Plugins"
copy /Y CrewRandR.version GameData\CrewRandR

cd GameData
mkdir "%H%\GameData\CrewRandR"
xcopy /y /s CrewRandR "%H%\GameData\CrewRandR"

