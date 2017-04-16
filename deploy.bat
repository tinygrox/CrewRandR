
set H=R:\KSP_1.2.2_dev
echo %H%

copy /Y "Source\bin\Debug\CrewQueue.dll" "GameData\CrewQueue\Plugins"
copy /Y CrewQueue.version GameData\CrewQueue

cd GameData
mkdir "%H%\GameData\CrewQueue"
xcopy /y /s CrewQueue "%H%\GameData\CrewQueue"

